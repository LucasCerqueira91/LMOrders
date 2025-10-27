# Documentação - Diagramas PlantUML

Esta pasta contém os arquivos PlantUML e as imagens geradas para a documentação do projeto.

## Arquivos

- **LMOrders-Modules.puml** - Diagrama de módulos e dependências
- **LMOrders-Folders.puml** - Estrutura de pastas do projeto
- **LMOrders-CQRS-CreateOrder.puml** - Fluxo CQRS para criação de pedido

## Regenerar Imagens

Para regenerar as imagens após modificar os arquivos `.puml`, execute:

```powershell
.\generate-diagrams.ps1
```

### Opção Manual

Se preferir gerar as imagens manualmente:

1. Acesse https://www.plantuml.com/plantuml/uml
2. Cole o conteúdo do arquivo `.puml`
3. Baixe a imagem gerada
4. Salve na pasta `docs/` com o mesmo nome

### Usando Java Localmente

Se você tiver o PlantUML instalado localmente:

```bash
java -jar plantuml.jar docs/*.puml
```

## Notas

- As imagens são geradas automaticamente pelo servidor online do PlantUML
- Formato: PNG
- Resolução: Qualidade padrão do servidor online
