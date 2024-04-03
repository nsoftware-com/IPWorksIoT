/*
 * IPWorks IoT 2022 .NET Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks IoT in a 
 * simple, straightforward way. It is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * www.nsoftware.com/ipworksiot
 *
 * This code is subject to the terms and conditions specified in the 
 * corresponding product license agreement which outlines the authorized 
 * usage and restrictions.
 * 
 */

using System.Collections.Generic;
﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksIoT;

class jsonDemo
{
  private static Json json = new nsoftware.async.IPWorksIoT.Json();

  static async Task Main(string[] args)
  {
    json.OnStartDocument += json_OnStartDocument;
    json.OnEndDocument += json_OnEndDocument;
    json.OnStartElement += json_OnStartElement;
    json.OnEndElement += json_OnEndElement;
    json.OnCharacters += json_OnCharacters;

    try
    {
      json.InputFile = "..\\..\\..\\books.json";
      await json.Parse();

      json.XPath = "/json/store/books";
      int bookCount = json.XChildren.Count;

      for (int i = 1; i <= bookCount; i++)
      {
        Console.WriteLine("\nBook #" + i);

        json.XPath = "/json/store/books/[" + i + "]";
        int propCount = json.XChildren.Count;

        for (int j = 1; j <= propCount; j++)
        {
          json.XPath = "/json/store/books/[" + i + "]/[" + j + "]";
          Console.WriteLine(json.XElement + ": " + json.XText);
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  #region "Events"

  private static void json_OnStartDocument(object sender, JsonStartDocumentEventArgs e)
  {
    Console.WriteLine("Started parsing file");
  }

  private static void json_OnEndDocument(object sender, JsonEndDocumentEventArgs e)
  {
    Console.WriteLine("Finished parsing file");
  }

  private static void json_OnStartElement(object sender, JsonStartElementEventArgs e)
  {
    Console.WriteLine("Element " + e.Element + " started");
  }

  private static void json_OnEndElement(object sender, JsonEndElementEventArgs e)
  {
    Console.WriteLine("Element " + e.Element + " ended");
  }

  private static void json_OnCharacters(object sender, JsonCharactersEventArgs e)
  {
    Console.WriteLine(e.Text);
  }

  #endregion
}


class ConsoleDemo
{
  public static Dictionary<string, string> ParseArgs(string[] args)
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();

    for (int i = 0; i < args.Length; i++)
    {
      // If it starts with a "/" check the next argument.
      // If the next argument does NOT start with a "/" then this is paired, and the next argument is the value.
      // Otherwise, the next argument starts with a "/" and the current argument is a switch.

      // If it doesn't start with a "/" then it's not paired and we assume it's a standalone argument.

      if (args[i].StartsWith("/"))
      {
        // Either a paired argument or a switch.
        if (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
        {
          // Paired argument.
          dict.Add(args[i].TrimStart('/'), args[i + 1]);
          // Skip the value in the next iteration.
          i++;
        }
        else
        {
          // Switch, no value.
          dict.Add(args[i].TrimStart('/'), "");
        }
      }
      else
      {
        // Standalone argument. The argument is the value, use the index as a key.
        dict.Add(i.ToString(), args[i]);
      }
    }
    return dict;
  }

  public static string Prompt(string prompt, string defaultVal)
  {
    Console.Write(prompt + (defaultVal.Length > 0 ? " [" + defaultVal + "]": "") + ": ");
    string val = Console.ReadLine();
    if (val.Length == 0) val = defaultVal;
    return val;
  }
}