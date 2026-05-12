# BusinersIA

BusinersIA é uma API desenvolvida em **.NET 10** voltada para a integração com inteligência artificial local utilizando o **Ollama**. 
A aplicação possui scripts de inicialização que detectam automaticamente se o Ollama e suas dependências (como o `zstd` em ambientes Linux/WSL) estão instalados, realizando a instalação caso necessário.

## 🚀 Como Iniciar

1. Certifique-se de ter o [SDK do .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) instalado.
2. Restaure as dependências do projeto e compile:
   ```bash
   dotnet build
   ```
3. Execute a aplicação:
   ```bash
   dotnet run
   ```
   *Nota: No Linux/WSL, se o script precisar instalar o Ollama ou o pacote `zstd`, pode ser necessário rodar o projeto como root (`sudo dotnet run`) caso o seu usuário exija senha para comandos `sudo`.*

---

## 🦙 Gerenciando Modelos no Ollama

O Ollama roda localmente na sua máquina. Abaixo estão os comandos básicos para gerenciar modelos no seu terminal:

- **Baixar e rodar um modelo**: `ollama run <nome_do_modelo>`
- **Apenas baixar (sem rodar)**: `ollama pull <nome_do_modelo>`
- **Listar modelos instalados**: `ollama list`
- **Remover um modelo**: `ollama rm <nome_do_modelo>`

---

## 📚 Modelos Recomendados

Abaixo estão listas de modelos recomendados para instalar no Ollama, divididos por categoria. A memória RAM recomendada refere-se ao uso de memória unificada (RAM normal ou VRAM de placa de vídeo).

### 💻 5 Modelos Focados em Programação (Coding)

Estes modelos são treinados especificamente em bilhões de linhas de código, sendo ideais para autocompletar código, encontrar bugs e criar scripts.

| Comando de Instalação | Modelo | Tamanho em Disco | Memória RAM Recomendada | Objetivo |
| :--- | :--- | :--- | :--- | :--- |
| `ollama run qwen2.5-coder` | **Qwen 2.5 Coder (7B)** | ~4.7 GB | 8 GB | Atualmente um dos melhores modelos open-source para código. Extremamente rápido e inteligente. |
| `ollama run codellama` | **Code Llama (7B)** | ~3.8 GB | 8 GB | Criado pela Meta com foco em geração e discussão de código estruturado. |
| `ollama run deepseek-coder-v2` | **DeepSeek Coder V2** | ~8.9 GB | 16 GB | Excelente raciocínio lógico em múltiplas linguagens. Um pouco mais pesado, mas com ótima precisão. |
| `ollama run phind-codellama` | **Phind CodeLlama** | ~19.0 GB | 24 GB | Modelo "fine-tuned" focado em responder dúvidas complexas de programação (estilo assistente sênior). |
| `ollama run starcoder2:3b` | **StarCoder 2 (3B)** | ~1.7 GB | 4 GB | Muito leve e rápido. Treinado em mais de 600 linguagens de programação, ótimo para hardwares modestos. |

### 🧠 5 Modelos de Uso Geral (General Purpose)

Estes modelos são ideais para conversação, tradução, resumos, análise de textos e tarefas cotidianas.

| Comando de Instalação | Modelo | Tamanho em Disco | Memória RAM Recomendada | Objetivo |
| :--- | :--- | :--- | :--- | :--- |
| `ollama run llama3.2` | **Llama 3.2 (3B)** | ~2.0 GB | 4 GB | Último modelo leve da Meta. Ótimo custo-benefício em precisão e uso baixíssimo de hardware. |
| `ollama run llama3.1` | **Llama 3.1 (8B)** | ~4.7 GB | 8 GB | O padrão atual da indústria para uso geral e conversação robusta (muito bom em português). |
| `ollama run phi3` | **Phi-3 Mini (3.8B)** | ~2.3 GB | 4 GB | Criado pela Microsoft. Pequeno, inteligente e roda incrivelmente rápido, superando até modelos maiores. |
| `ollama run gemma2:9b` | **Gemma 2 (9B)** | ~5.4 GB | 8 a 12 GB | Desenvolvido pelo Google baseado na tecnologia do Gemini. Ótimo raciocínio lógico e coerência. |
| `ollama run mistral` | **Mistral (7B)** | ~4.1 GB | 8 GB | Clássico modelo de código aberto. Versátil, excelente fluência e adaptação rápida às instruções do usuário (prompts). |

Criação de model-agent:  ollama create super-coder -f Agent/Modelfile
