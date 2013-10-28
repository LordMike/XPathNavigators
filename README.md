# XPathNavigators
A set of XPathNavigator implementations for various purposes, as well as an easy-to-use wrapper which will help developers create their own navigators.

## Purpose

Personally, I wanted to perform XPath on the windows registry. But it just wasn't possible, without doing a lot of work. So with a lot of help from [Codeproject](http://www.codeproject.com/Articles/438976/Using-XPath-to-Navigate-the-File-System), a little luck and some debugging, I've been able to make some prototype navigators and a small framework.

## Nuget

Maybe later.

## Features

* Different navigators, which can be altered, extended or used as inspiration.
* 'Framework' which helps developing navigators.
* Prototype Navigators for:
 *  Registry
 *  Offreg (Offline registry)
 *  Filesystem

## Notes

* I haven't done a lot of structured testing, so there may (probably will be) bugs of all kinds.
* I've included a copy of AlphaFS, which I've rewritten a little to make it work in .NET 2.0, so that I may keep the lowest common denominator down there. It is totally possible to just use the .NET 4 version [directly from the source](http://alphafs.codeplex.com/).

## Examples 
For examples, look at the implemented navigators on how to proceed with f.ex. the Filesystem or the Registry.