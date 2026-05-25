## First Run

After starting the services, pull the Ollama model:

```bash
docker exec -it closedcv-ollama ollama pull llama3.2:3b
```

This only needs to be done once — the model is stored in the `ollama_data` volume.
