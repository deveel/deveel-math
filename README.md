Deveel Math
===========
[![Build status](https://ci.appveyor.com/api/projects/status/ut1oyratnbqo5yg5?svg=true)](https://ci.appveyor.com/project/tsutomi/deveel-math)

[![Join the chat at https://gitter.im/deveel/deveel-math](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/deveel/deveel-math?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This is the port of the Java Math library implemented by the Apache Harmony framework, that is used to factorize big numbers and decimals, for the .NET and Mono frameworks.
In fact the native .NET support for decimal numbers appear to be limited in several contexts, leading some independent developments of the support within applications.

Tha aim of this library is to provide .NET developers with a powerful instrument to handle operations on very big numbers, keeping performances and reliability under control.


How to Install It
==================

From the NuGet Package Management console, select the project where the library will be installed and type the following command

```
PM> Install-Package dmath
```
