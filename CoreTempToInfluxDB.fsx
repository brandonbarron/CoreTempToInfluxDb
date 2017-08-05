#r "FSharp.Data.dll"
open FSharp.Data
open System
open System.Text
open System.IO
open System.Net

let fileLoc = "C:\Users\Brandon\Downloads\RealTemp_370\RealTempLog.csv"

let url = "http://posttestserver.com/post.php" //"http://192.168.1.26:8086/write?db=home"

let postDocRaw (url:string) (data: string) : string =
      let data' : byte[] = System.Text.Encoding.ASCII.GetBytes(data);

      let request = WebRequest.Create(url)
      request.Method        <- "POST"
      request.ContentType   <- "application/x-www-form-urlencoded"
      request.ContentLength <- (int64) data'.Length

      use wstream = request.GetRequestStream() 
      wstream.Write(data',0, (data'.Length))
      wstream.Flush()
      wstream.Close()

      let response  = request.GetResponse()
      use reader     = new StreamReader(response.GetResponseStream())
      let output = reader.ReadToEnd()

      reader.Close()
      response.Close()
      request.Abort()

      output

let tempFile = CsvFile.Load(fileLoc).Cache()

let readFile = 
    for row in tempFile.Rows do
        printfn "Load = %s; CPU_0 = %s" (row.GetColumn("LOAD%")) (row.GetColumn("CPU_0")) |> ignore
        let dataString = String.Format("Load = {0}, CPU_0 = {1}", row.GetColumn("LOAD%"), row.GetColumn("CPU_0"))
        printfn "response: %A" (postDocRaw url dataString)

System.Console.ReadLine() |> ignore