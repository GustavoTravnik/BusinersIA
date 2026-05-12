# API Tests para os Endpoints do Ollama

## 1. Iniciar o processamento de uma resposta (POST /getResponse)
Este endpoint recebe o `Input` (prompt) e o `ModelName` e inicia o processo em segundo plano, retornando o ID do processo.

```bash
curl -X POST "http://localhost:5123/getResponse" \
     -H "Content-Type: application/json" \
     -d '{"input": "Me explique o que é Inteligência Artificial em uma frase.", "modelName": "llama3.2"}'
```

**Exemplo de Resposta:**
```json
{
  "processId": "d3b07384-d113-4e12-990f-5649467614e5"
}
```

---

## 2. Consultar o resultado do processamento (GET /getResponseResult/{id})
Utilize o `processId` retornado no passo anterior para verificar o status e obter a resposta do modelo quando concluído. Substitua `{id}` pelo ID real.

```bash
curl -X GET "http://localhost:5123/getResponseResult/d3b07384-d113-4e12-990f-5649467614e5"
```

**Exemplo de Resposta (Em Processamento):**
```json
{
  "id": "d3b07384-d113-4e12-990f-5649467614e5",
  "status": "Processing",
  "content": null
}
```

**Exemplo de Resposta (Concluído):**
```json
{
  "id": "d3b07384-d113-4e12-990f-5649467614e5",
  "status": "Completed",
  "content": "A Inteligência Artificial é a simulação de processos de inteligência humana por máquinas, especialmente sistemas de computador."
}
```
