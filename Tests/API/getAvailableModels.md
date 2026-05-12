# API Test para getAvailableModels

## Obter modelos disponíveis (GET /getAvailableModels)
Este endpoint retorna uma lista em formato JSON com todos os modelos que já estão instalados no Ollama localmente.

```bash
curl -X GET "http://localhost:5123/getAvailableModels"
```

**Exemplo de Resposta:**
```json
{
  "models": [
    {
      "name": "llama3.2:latest",
      "model": "llama3.2:latest",
      "modified_at": "2023-11-20T10:00:00.0000000Z",
      "size": 2011000000,
      "digest": "abcdef1234567890",
      "details": {
        "parent_model": "",
        "format": "gguf",
        "family": "llama",
        "families": ["llama"],
        "parameter_size": "3B",
        "quantization_level": "Q4_0"
      }
    }
  ]
}
```
