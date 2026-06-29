# ✂️ SplitMergePdf

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white)
![iText7](https://img.shields.io/badge/iText7-9.6-007E33?style=flat-square)
![Blog](https://img.shields.io/badge/blog-rsrsystem-FF5722?style=flat-square&logo=blogger&logoColor=white)

> Librería **.NET 10** para **dividir y unir archivos PDF** desde PowerBuilder, con [iText7](https://github.com/itext/itext-dotnet).

## 📋 ¿Qué es esto?

La librería que uso en el ejemplo de PowerBuilder **pbfileservice** para **partir** un PDF en varios
o **juntar** varios en uno solo. Se apoya en **iText7** y se consume desde PB como un `dotnetobject`.

## 🧩 Dependencias

| Paquete | Versión |
|---------|---------|
| [itext7](https://www.nuget.org/packages/itext7) | `9.6.0` |

> 🆕 **Migración a .NET 10:** actualizado de **iText7 8.0.5** a **9.6.0**.

## 🛠️ Requisitos

- **.NET SDK 10.0** o superior

## 🚀 Compilar

```bat
dotnet build SplitMergePdf.csproj -c Release
```

La DLL queda en `bin\Release\net10.0\`.

## 🔗 Proyecto PowerBuilder relacionado

👉 **pbfileservice** — https://github.com/rasanfe/pbfileservice

---

📨 **Blog:** <https://rsrsystem.blogspot.com/>

> ¡Nos vemos en el próximo artículo! Y recuerda: en PowerBuilder, los límites solo están en nuestra imaginación. 🚀
