# Utilities.Core.Shared80

Utilities.Core.Shared80 es una librería de utilidades para .NET 8 orientada a mejorar la productividad y reutilización de funciones comunes en proyectos empresariales.  
Incluye herramientas para manejo de datos, encriptación, validación, conversión de tipos, hashing, y más.

---

## 🚀 Características principales

- Conversión entre tipos y manejo de enumeraciones
- Helpers para hashing SHA256 y SHA512
- Mapeo de DataReader a listas fuertemente tipadas
- Conversión UnixTimestamp ↔ DateTime
- Validaciones comunes y extensiones
- Utilidades generales para cadenas
- Funciones auxiliares para reflección
- Normalización de conexión por URL

---

## 📦 Instalación

```bash
dotnet add package Utilities.Core.Shared80
```

o en el archivo `.csproj`:

```xml
<PackageReference Include="Utilities.Core.Shared80" Version="1.0.0" />
```

---

## 📘 Uso básico

### Obtener valores de enum por descripción

```csharp
var value = Functions.GetEnumValueFromDescription<MyEnum>("Activo");
```

### Generar Hash SHA512

```csharp
var (hex, base64) = Functions.GenerateHash512("mypassword");
```

### Mapear un DataReader a una lista

```csharp
var list = Functions.DataReaderMapToListAsync<MyDto>(reader);
```

### Convertir Timestamp de Linux

```csharp
var date = Functions.GetDateFromLinuxDateTime(1609459200);
```

---

## 🧪 Pruebas unitarias

Incluye pruebas para:

- Hashing
- Enum mapping
- Timestamp conversion
- Generación de strings aleatorios
- Mapeo de DataReader
- Búsqueda de excepciones por namespace

---

## ✅ Compatibilidad

- .NET 8 (probado)
- .NET 6/7 (compatible)
- Windows, Linux, macOS

---

## 🛠️ Requisitos

No se requieren dependencias externas adicionales.

---

## 📄 Licencia

Este paquete usa licencia MIT.  
Consulta el archivo `LICENSE` incluido en el paquete.

---

## ❤️ Contribuciones

Las contribuciones son bienvenidas. Para mejoras o propuestas crea un pull request o abre un issue.

---

## 📧 Contacto

Para soporte o dudas:  
**support@hogar.com**  
o abre un issue en el repositorio.
