﻿module Mavnn.MythMaker.MythProvider4

open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection

[<TypeProvider>]
type MythProvider4() as this =
    inherit TypeProviderForNamespaces()

    let ns = "Mavnn.MythMaker.MythTypes4"
    let asm = Assembly.GetExecutingAssembly()

    let createType typeName (args : obj []) =
        let myString = args.[0] :?> string
        let staticProp =
            ProvidedProperty(
                "StaticProperty",
                typeof<string>, 
                IsStatic = true,
                GetterCode = fun _ -> <@@ myString @@>)
        let instanceProp =
            ProvidedProperty(
                "InstanceProperty",
                typeof<string>,
                GetterCode = fun args -> <@@ unbox (%%args.[0]:obj) @@>)
        let ctor =
            ProvidedConstructor(
                [ProvidedParameter("Instance string", typeof<string>)],
                InvokeCode = fun args -> <@@ (%%args.[0]:string) @@>)
        let myType = 
            ProvidedTypeDefinition(asm, ns, typeName, Some typeof<obj>)
        myType.AddMember ctor
        myType.AddMembers [staticProp;instanceProp]
        myType

    let provider = ProvidedTypeDefinition(asm, ns, "MythProvider", Some typeof<obj>)
    let parameters =
        [ProvidedStaticParameter("AString", typeof<string>)]

    do
        provider.DefineStaticParameters(parameters, createType)
        this.AddNamespace(ns, [provider])

(*
#r @"Mavnn.MythMaker/bin/Debug/Mavnn.MythMaker.dll"

open Mavnn.MythMaker.MythTypes4

type OurType = MythProvider<"A string here">

let ourType = OurType("Instance String!")

ourType.InstanceProperty
*)