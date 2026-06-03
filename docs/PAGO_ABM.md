# ABM de Pagos - Documentación

## Overview
El módulo de Pagos permite registrar, consultar, modificar y eliminar pagos realizados por los participantes en un viaje. Sigue la arquitectura de capas del proyecto y proporciona una REST API completa.

## Estructura del Módulo

### 1. Domain Layer (Entidades)
**Archivo**: `src/Domain/Entities/Pago.cs`

```csharp
public class Pago
{
    public int Id { get; set; }
    public int ParticipanteId { get; set; }
    public int ViajeId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Metodo { get; set; }
    public string Comprobante { get; set; }
    
    // Relaciones
    public ParticipanteViaje Participante { get; set; }
    public Viaje Viaje { get; set; }
}
```

### 2. Domain Layer (Interfaces)
**Archivo**: `src/Domain/Interfaces/IPagoRepository.cs`

Define los métodos para acceso a datos:
- `GetAll()` - Obtiene todos los pagos
- `GetById(int id)` - Obtiene un pago por ID
- `Add(Pago entity)` - Crea un nuevo pago
- `Update(Pago entity)` - Actualiza un pago existente
- `Delete(int id)` - Elimina un pago
- `GetByViajeId(int viajeId)` - Obtiene pagos de un viaje específico
- `GetByParticipanteId(int participanteId)` - Obtiene pagos de un participante específico

### 3. Infrastructure Layer
**Archivo**: `src/Infrastructure/Data/PagoRepository.cs`

Implementa la interfaz `IPagoRepository` con operaciones CRUD contra la base de datos SQLite.

**Archivo**: `src/Infrastructure/Data/ApplicationContext.cs`

Se agregó el DbSet:
```csharp
public DbSet<Pago> Pagos { get; set; }
```

### 4. Application Layer

#### Servicio
**Archivo**: `src/Application/Services/PagoService.cs`

Contiene la lógica de negocio:
- Validación de pagos
- Transformación de entidades a DTOs
- Métodos para obtener pagos por viaje o participante

#### Interfaces
**Archivo**: `src/Application/Interfaces/IPagoService.cs`

Define los métodos del servicio accesibles desde los controladores.

#### DTOs
**Archivo**: `src/Application/Models/PagoDto.cs`

DTO para respuestas:
```csharp
public class PagoDto
{
    public int Id { get; set; }
    public int ParticipanteId { get; set; }
    public int ViajeId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Metodo { get; set; }
    public string Comprobante { get; set; }
}
```

**Archivo**: `src/Application/Models/Requests/PagoRequest.cs`

DTO para solicitudes:
```csharp
public class PagoRequest
{
    public int ParticipanteId { get; set; }
    public int ViajeId { get; set; }
    public decimal Monto { get; set; }
    public string Metodo { get; set; }
    public string Comprobante { get; set; }
}
```

### 5. Web Layer
**Archivo**: `src/Web/Controllers/PagoController.cs`

Proporciona los siguientes endpoints REST:

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/pago` | Crear un nuevo pago |
| GET | `/api/pago` | Obtener todos los pagos |
| GET | `/api/pago/{id}` | Obtener un pago por ID |
| PUT | `/api/pago/{id}` | Actualizar un pago |
| DELETE | `/api/pago/{id}` | Eliminar un pago |
| GET | `/api/pago/viaje/{viajeId}` | Obtener pagos de un viaje |
| GET | `/api/pago/participante/{participanteId}` | Obtener pagos de un participante |

## Validaciones

El servicio valida:
- `ParticipanteId` debe ser mayor a 0
- `ViajeId` debe ser mayor a 0
- `Monto` debe ser mayor a 0
- `Metodo` no puede estar vacío

## Ejemplos de Uso

### Crear un Pago
```json
POST /api/pago
Content-Type: application/json
Authorization: Bearer {token}

{
    "participanteId": 1,
    "viajeId": 1,
    "monto": 500.00,
    "metodo": "Transferencia Bancaria",
    "comprobante": "/comprobantes/pago-123.pdf"
}

Response (201 Created):
{
    "id": 1,
    "participanteId": 1,
    "viajeId": 1,
    "monto": 500.00,
    "fecha": "2026-06-03T18:15:30",
    "metodo": "Transferencia Bancaria",
    "comprobante": "/comprobantes/pago-123.pdf"
}
```

### Obtener Todos los Pagos
```
GET /api/pago
Authorization: Bearer {token}
```

### Obtener Pagos de un Viaje
```
GET /api/pago/viaje/1
Authorization: Bearer {token}
```

### Actualizar un Pago
```json
PUT /api/pago/1
Content-Type: application/json
Authorization: Bearer {token}

{
    "participanteId": 1,
    "viajeId": 1,
    "monto": 550.00,
    "metodo": "Transferencia Bancaria",
    "comprobante": "/comprobantes/pago-123-actualizado.pdf"
}
```

### Eliminar un Pago
```
DELETE /api/pago/1
Authorization: Bearer {token}
```

## Registración en Program.cs

El módulo está registrado en `src/Web/Program.cs`:

```csharp
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IPagoService, PagoService>();
```

## Migración de Base de Datos

La migración `CreatePagoTable` crea la tabla `Pagos` con:
- Clave primaria: `Id`
- Claves foráneas: `ParticipanteId` (ParticipantesViaje) y `ViajeId` (Viajes)
- Índices en `ParticipanteId` y `ViajeId`
- Eliminación en cascada habilitada

## Seguridad

Todos los endpoints requieren autenticación JWT mediante el atributo `[Authorize]`.

## Relaciones

- `Pago.Participante` → Relación con `ParticipanteViaje`
- `Pago.Viaje` → Relación con `Viaje`
- `ParticipanteViaje.Pagos` → Colección inversa de `Pago`
- `Viaje.Pagos` → Colección inversa de `Pago`

