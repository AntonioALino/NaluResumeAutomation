from fastapi import FastAPI, UploadFile, File, HTTPException
from pydantic import BaseModel
import fitz  
import json
import re
import os
from dotenv import load_dotenv

from langchain_core.prompts import PromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_google_genai import ChatGoogleGenerativeAI

load_dotenv()

app = FastAPI(title="StudyBuddy AI Worker")

class ProcessPdfResult(BaseModel):
    summaryText: str
    mindMapMarkdown: str

def extract_text_from_pdf(file_bytes: bytes) -> str:
    text = ""
    with fitz.open(stream=file_bytes, filetype="pdf") as doc:
        for page in doc:
            text += page.get_text()
    return text

def generate_study_material(text: str) -> dict:
    try:
        llm = ChatGoogleGenerativeAI(
            model="gemini-3.5-flash", 
            temperature=0.3,
            response_mime_type="application/json" 
        )
        
        prompt = PromptTemplate.from_template(
            """Você é um assistente acadêmico especialista em resumos.
            Analise o texto abaixo e retorne APENAS um objeto JSON válido com as seguintes chaves:
            1. "summaryText": Um resumo claro, direto e estruturado do conteúdo.
            2. "mindMapMarkdown": O texto estruturado no formato Markdown Mermaid (grafico de nós) representando um mapa mental do conteúdo.
            
            Texto do PDF:
            {texto}
            """
        )
        
        chain = prompt | llm | StrOutputParser()
        
        response = chain.invoke({"texto": text}) 
        
        # --- BLINDAGEM ANTI-LIXO (REGEX) ---
        # Procura o primeiro '{' e o último '}', ignorando o resto do texto
        match = re.search(r'\{.*\}', response, re.DOTALL)
        
        if match:
            clean_json_string = match.group(0)
            return json.loads(clean_json_string)
        else:
            # Fallback caso a IA não retorne chaves (muito raro)
            return json.loads(response)
        
        return json.loads(response)
        
    except Exception as api_error:
        # Isso vai printar o erro REAL do Gemini no seu terminal Python
        print(f"\n[ERRO CRÍTICO NO GEMINI]: {str(api_error)}\n")
        raise api_error

@app.post("/api/process-pdf", response_model=ProcessPdfResult)
async def process_pdf(file: UploadFile = File(...)):
    try:
        file_bytes = await file.read()
        
        text = extract_text_from_pdf(file_bytes)
        if not text.strip():
            raise HTTPException(status_code=400, detail="Não foi possível extrair texto deste PDF. Ele pode ser uma imagem escaneada.")
        
        ai_result = generate_study_material(text)
        
        return ProcessPdfResult(
            summaryText=ai_result.get("summaryText", "Resumo não gerado."),
            mindMapMarkdown=ai_result.get("mindMapMarkdown", "Mapa mental não gerado.")
        )
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))