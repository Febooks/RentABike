# RentABike.Tests

Projeto de testes unitários para a aplicação RentABike.

## Estrutura

```
RentABike.Tests/
├── Domain/
│   └── Entities/
│       ├── RentalTests.cs
│       ├── MotorcycleTests.cs
│       └── DeliveryPersonTests.cs
└── README.md
```

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **FluentAssertions**: Biblioteca para assertions mais legíveis
- **Moq**: Framework para criação de mocks
- **AutoMapper**: Para testes de mapeamento (se necessário)

## Executando os Testes

### Executar todos os testes
```bash
dotnet test
```

### Executar testes de um arquivo específico
```bash
dotnet test --filter FullyQualifiedName~RentalTests
```

### Executar com cobertura de código
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Executar com output detalhado
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Testes Implementados

### RentalTests
- `RegisterReturn_EarlyReturn_7DaysPlan_ShouldCalculateFine`: Testa cálculo de multa para plano de 7 dias
- `RegisterReturn_EarlyReturn_15DaysPlan_ShouldCalculateFine`: Testa cálculo de multa para plano de 15 dias
- `RegisterReturn_LateReturn_ShouldCalculateAdditionalAmount`: Testa cálculo de valor adicional para devolução atrasada
- `RegisterReturn_OnTime_ShouldNotCalculateFineOrAdditional`: Testa devolução no prazo
- `RegisterReturn_EarlyReturn_30DaysPlan_ShouldNotCalculateFine`: Testa que planos diferentes de 7 e 15 dias não têm multa
- `CalculateDailyRate_ValidPlans_ShouldReturnCorrectRate`: Testa cálculo de diária para diferentes planos

### MotorcycleTests
- `UpdateLicensePlate_ValidPlate_ShouldUpdate`: Testa atualização de placa válida
- `UpdateLicensePlate_EmptyPlate_ShouldThrowException`: Testa validação de placa vazia
- `UpdateLicensePlate_WhiteSpacePlate_ShouldThrowException`: Testa validação de placa com espaços
- `Constructor_ValidData_ShouldCreateMotorcycle`: Testa criação de moto

### DeliveryPersonTests
- `CanRent_LicenseTypeA_ShouldReturnTrue`: Testa permissão de aluguel com CNH tipo A
- `CanRent_LicenseTypeAB_ShouldReturnTrue`: Testa permissão de aluguel com CNH tipo AB
- `CanRent_LicenseTypeB_ShouldReturnFalse`: Testa que CNH tipo B não pode alugar
- `UpdateLicenseImage_ValidUrl_ShouldUpdate`: Testa atualização de imagem da CNH
- `Constructor_ValidData_ShouldCreateDeliveryPerson`: Testa criação de entregador

## Adicionando Novos Testes

1. Crie uma nova classe de teste seguindo o padrão `*Tests.cs`
2. Use `[Fact]` para testes únicos ou `[Theory]` com `[InlineData]` para testes parametrizados
3. Use FluentAssertions para assertions mais legíveis
4. Use Moq para criar mocks quando 