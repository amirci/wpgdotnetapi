namespace WpgDotNet
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/Suave/lib/net40/Suave.dll"

open Suave
open Suave.Web
open System
open System.Text
open FSharp.Data
open Suave.Logging

module Commons =
    exception SlackException of string

    let [<Literal>] exceptionSample = """ { "message":"friendly error" } """
    type ExceptionJson = JsonProvider<exceptionSample, RootName="Root">

    let FiveHundred(msg: string) (ctx : HttpContext) = 
      Response.response HTTP_500 (Encoding.UTF8.GetBytes (msg)) ctx

    let processError (ex: Exception) (ctx : HttpContext)= 
      match ex with 
      | SlackException(s) -> FiveHundred (ExceptionJson.Root(message=s).JsonValue.ToString()) ctx
      |_ -> 
        let internalErrText = ExceptionJson.Root(message=HTTP_500.message).JsonValue.ToString()
        Response.response HTTP_400 (Encoding.UTF8.GetBytes (internalErrText)) ctx
