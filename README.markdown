# ApplicationHost

LGPL License.

## Introduction

The ApplicationHost project is a prototype implementation for hosting .NET
applications in a user control.

The idea for implementing this functionality came from the [roozz hosted
LINQPad instance](http://prod.roozz.com/apps/61/LINQPad.htm).
[Roozz](http://www.roozz.com/) provides a mechanism for running .NET applications
inside a Google Chrome page. The ApplicationHost project attempts to replicate
this functionality, but instead of hosting the application inside a Google
Chrome tab, it hosts it in a user control.

## Usage

Using the ApplicationHost user control involves two steps:

1. Place the AppHost control on a form;
2. Call the StartApplication method with the path to the executable to execute.

These basic steps are enough to get yourself up and running.

## Customizations

ApplicationHost works by hooking into Windows and watching for newly created
windows. When the application host finds a new window, it transforms so that
it is hosted in the user control instead of like a normal application. This
mechanism is implemented using window filters. A window filter is a class
that derives from the WindowFilter class. An implementation provides
logic for matching a newly created window against some criteria. When it is
matched, the filter will be used to attach the window to the application host,
resize it and detach it from the application host.

The current implementation has been tested with [Reflector](http://www.reflector.net/)
and [LINQPad](http://www.linqpad.net/). When you want to host an application which
is not properly handled by the default filters, you can implement your
own window filter by deriving from the WindowFilter class. Refer to the
existing implementations for information on how to implement a custom WindowFilter.

The primary mechanism with which windows are matched is by
[WindowStyle](http://msdn.microsoft.com/en-us/library/windows/desktop/ms632600.aspx) and
[windowStyleEx](http://msdn.microsoft.com/en-us/library/windows/desktop/ff700543.aspx).
The easiest way to discover the styles of a window is to use
[Spy++](http://msdn.microsoft.com/en-us/library/aa264396.aspx). This should be installed
automatically with your Visual Studio installation.

To use a custom WindowFilter, add an instance of your custom filter to the
WindowFilters collection before starting the application.

## License

ApplicationHost is licensed under the LGPL 3.
