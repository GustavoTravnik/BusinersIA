# API Test para enableModel

## Instalar um modelo (POST /enableModel)
Este endpoint recebe o parâmetro `modelName` na query (ou corpo dependendo da forma como o .NET vinculou o parâmetro primitivo, mas no nosso caso ele está no query string por padrão devido à assinatura `string modelName`) e aciona o comando `ollama pull` em background.

```bash
curl -X POST "http://localhost:5123/enableModel?modelName=gemma:2b" \
     -H "Content-Type: application/json"
```

**Exemplo de Resposta:**
```json
{
  "message": "Started installing model 'gemma:2b' via ollama pull."
}
```
