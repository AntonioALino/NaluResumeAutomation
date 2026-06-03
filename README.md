# 📚 NaluResumeAutomation

> Bot para Telegram que transforma PDFs em resumos estruturados e mapas mentais gerados por IA — automaticamente.

---

## ✨ Sobre o Projeto

O **NaluResumeAutomation** nasceu de uma necessidade real: ajudar no estudo com PDFs acadêmicos de forma mais eficiente. Basta enviar um arquivo PDF no Telegram e, em instantes, você recebe:

- ✅ **Resumo estruturado** do conteúdo do documento
- ✅ **Mapa mental gerado por IA** em formato visual
- ✅ **Imagem PNG** pronta para visualização no celular

Tudo com apenas um envio de arquivo no chat.

---

## 🏗️ Arquitetura

O projeto é composto por múltiplos serviços desacoplados, cada um com responsabilidade bem definida:

```
┌─────────────────┐        ┌──────────────────────┐
│  Telegram User  │──PDF──▶│  Backend Orquestrador │
└─────────────────┘        │    (.NET / C#)         │
                           └──────────┬───────────┘
                                      │
               ┌──────────────────────┼──────────────────────┐
               ▼                      ▼                       ▼
  ┌─────────────────────┐  ┌──────────────────────┐  ┌──────────────────┐
  │  Microsserviço de   │  │   Camada Cognitiva    │  │   Renderização   │
  │  Processamento      │  │  Google Gemini +      │  │     Visual       │
  │  (Python + FastAPI) │  │    LangChain          │  │     (Kroki)      │
  └─────────────────────┘  └──────────────────────┘  └──────────────────┘
```

### 🔹 Backend Orquestrador — `.NET / C#`

O maestro da aplicação, construído com **Clean Architecture** e princípios **SOLID**. Responsável por:

- Receber e processar eventos do Telegram
- Baixar os arquivos enviados pelos usuários
- Coordenar a comunicação entre os serviços
- Aplicar regras de negócio
- Garantir resiliência e tratamento de falhas no fluxo

### 🔹 Microsserviço de Processamento — `Python + FastAPI`

Responsável pela extração e preparação dos dados brutos. Utiliza **PyMuPDF** para:

- Leitura e parsing dos arquivos PDF
- Preparação do conteúdo textual para envio à IA

### 🔹 Camada Cognitiva — `Google Gemini + LangChain`

O cérebro da solução. O modelo de linguagem interpreta o conteúdo extraído e gera uma estrutura padronizada via **schema JSON** contendo:

- Resumo organizado do documento
- Mapa mental em formato **Mermaid**

### 🔹 Renderização Visual — `Kroki`

Converte o código Mermaid gerado pela IA em uma **imagem PNG**, entregue diretamente ao usuário no Telegram.

---

## 🛠️ Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Bot / Interface | Telegram Bot API |
| Backend / Orquestrador | .NET / C# (Clean Architecture) |
| Processamento de PDF | Python, FastAPI, PyMuPDF |
| IA Generativa | Google Gemini, LangChain |
| Renderização de Diagramas | Kroki (Mermaid → PNG) |

---

## ⚡ Como Funciona

```
1. Usuário envia PDF no Telegram
        ↓
2. Orquestrador recebe o evento e baixa o arquivo
        ↓
3. Microsserviço Python extrai o texto com PyMuPDF
        ↓
4. Google Gemini processa o conteúdo e gera resumo + Mermaid
        ↓
5. Kroki converte o Mermaid em imagem PNG
        ↓
6. Usuário recebe resumo + mapa mental no Telegram
```

---

## 🧠 Desafios Técnicos

Integrar IA foi apenas parte do trabalho. Os maiores desafios foram de engenharia:

- **Timeouts** para processamento de documentos extensos
- **Tratamento de falhas** na comunicação entre microsserviços
- **Parsing de mensagens** compatível com a API do Telegram
- **Estratégias de fallback** para garantir entrega mesmo com indisponibilidades externas

> *IA gera valor quando está apoiada por uma arquitetura sólida e bem definida.*

---

## 🔮 Próximos Passos

- [ ] 🃏 Geração de **flashcards automáticos** para revisão espaçada
- [ ] 🧪 **Quiz inteligente** baseado no conteúdo do PDF
- [ ] 📊 Resumos em **diferentes níveis de profundidade** (rápido, intermediário e completo)
- [ ] 🗂️ Exportação dos mapas mentais para **Notion** e **Obsidian**
- [ ] 📁 **Histórico de documentos** processados
- [ ] 💬 **Chat contextual** para tirar dúvidas sobre o material enviado
- [ ] 📦 Processamento de **múltiplos arquivos** e consolidação de conteúdos
- [ ] 🎧 **Áudio-resumo** para estudo durante deslocamentos

---

## 📁 Estrutura do Projeto

```
NaluResumeAutomation/
├── src/                       # Backend .NET — Clean Architecture
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── API/
│   Python-Worker/          # Microsserviço Python + FastAPI
│       └── main.py
└── README.md
```

---

## 🚀 Como Executar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Python 3.10+](https://www.python.org/)
- [Docker](https://www.docker.com/) (recomendado)
- Conta no [Telegram](https://telegram.org/) e um Bot Token (via [@BotFather](https://t.me/BotFather))
- Chave de API do [Google Gemini](https://ai.google.dev/)

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz com as seguintes variáveis:

```env
TELEGRAM_BOT_TOKEN=seu_token_aqui
GEMINI_API_KEY=sua_chave_aqui
PYTHON_WORKER_URL=http://pdf-processor:8000
```

### Manual

```bash
# Microsserviço Python
cd src/PdfProcessor
pip install -r requirements.txt
uvicorn main:app --reload

# Backend .NET
cd src/Orchestrator
dotnet restore
dotnet run
```

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir uma _issue_ ou enviar um _pull request_.

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/minha-feature`)
3. Faça commit das suas alterações (`git commit -m 'feat: minha nova feature'`)
4. Faça push para a branch (`git push origin feature/minha-feature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

<div align="center">
  Feito por Antônio Lino com 💙 para tornar o estudo mais inteligente
</div>
