
#r "C:\Users\Rick\Documents\My Dropbox\Projects\F# Group Scraper\Group Attendance\Group Attendance\HtmlAgilityPack.dll"

open System
open System.IO
open System.Text
open HtmlAgilityPack

do Directory.SetCurrentDirectory (@"C:\Users\Rick\Documents\My Dropbox\Projects\F# Group Scraper\Group Attendance\Group Attendance\")
let doc = new HtmlDocument()
do doc.Load("Meetup.com › RSVP List  Double Trouble with Paulmichael Blasucci and Steve Goguen.htm")

///Fullname /html/body/div/div[5]/table/tbody/tr[20]/td[4]/dl/dd
///Username /html/body/div/div[5]/table/tbody/tr[23]/td/span
let allUsers = 
    [
        for entity in doc.DocumentNode.SelectNodes("/html/body/div/div[5]/table/tbody/tr[*]") do
            let fullname = 
                let fullnamenode = entity.SelectSingleNode("./td[4]/dl/dd")
                if fullnamenode <> null && fullnamenode.InnerText.Trim().Contains(" ") then fullnamenode.InnerText
                else entity.SelectSingleNode("./td/span").InnerText
            yield fullname.Replace("\"", "").Split([|Environment.NewLine|],  StringSplitOptions.RemoveEmptyEntries).[0].Trim()
    ]

// Split out those bringing others
let actualUsers =
    [
        for user in allUsers do
            yield! user.Split([|','|]) |> List.ofArray |> List.map (fun u -> u.Trim())
    ]

// Show Count to make sure nothing got messed up
actualUsers |> List.length

// Show users for visual inspection
actualUsers |> Seq.iter (fun x -> printfn "%A" x)

// Output CSV
let outputText = new StringBuilder()
actualUsers
|> List.map (fun n -> let index = n.IndexOf(" ") in
                      if index >= 0 then n.Substring(0, index), n.Substring(index + 1)
                      else n, "")
|> List.iter (fun (n1, n2) -> outputText.AppendLine(String.Format("{0},{1}", n1, n2)) |> ignore)

//Final output
printfn "%s" (outputText.ToString())

File.WriteAllText(@"C:\Temp\group-attendees-9-06-2011.csv", outputText.ToString()) |> ignore
