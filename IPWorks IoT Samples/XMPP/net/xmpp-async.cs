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

class xmppDemo
{
  private static Xmpp xmpp = new nsoftware.async.IPWorksIoT.Xmpp();
  private static bool messageReceived = false;

  static async Task Main(string[] args)
  {
    if (args.Length < 6)
    {
      Console.WriteLine("usage: xmpp /s server /u username /p password\n");
      Console.WriteLine("  server      the name or address of the XMPP server");
      Console.WriteLine("  username    the username used to authenticate to the XMPP server");
      Console.WriteLine("  password    the password used to authenticate to the XMPP server");
      Console.WriteLine("\nExample: xmpp /s talk.google.com /u myusername /p mypassword");
    }
    else
    {
      xmpp.OnSSLServerAuthentication += xmpp_OnSSLServerAuthentication;
      xmpp.OnConnected += xmpp_OnConnected;
      xmpp.OnDisconnected += xmpp_OnDisconnected;
      xmpp.OnMessageIn += xmpp_OnMessageIn;

      try
      {
        Dictionary<string, string> myArgs = ConsoleDemo.ParseArgs(args);

        xmpp.IMServer = myArgs["s"];
        xmpp.User = myArgs["u"];
        xmpp.Password = myArgs["p"];

        Console.WriteLine("Connecting...");
        await xmpp.Connect();

        int buddyCount = xmpp.Buddies.Count;
        Console.WriteLine("Buddy list:");

        if (buddyCount > 0)
        {
          for (int i = 0; i < buddyCount; i++)
          {
            Console.WriteLine(i + 1 + ") " + xmpp.Buddies[i].Id);
          }

          Console.Write("Select a buddy: ");
          int buddy = int.Parse(Console.ReadLine());

          Console.Write("Message to send to buddy: ");
          xmpp.MessageText = Console.ReadLine();

          await xmpp.SendMessage(xmpp.Buddies[buddy - 1].Id);

          Console.WriteLine("Receiving responses...");
          while (!messageReceived)
          {
            await xmpp.DoEvents();
          }

          Console.WriteLine("Disconnecting...");
          await xmpp.Disconnect();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }

  #region "Events"

  private static void xmpp_OnSSLServerAuthentication(object sender, XmppSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  private static void xmpp_OnConnected(object sender, XmppConnectedEventArgs e)
  {
    Console.WriteLine("Welcome! Connection established!");
  }

  private static void xmpp_OnDisconnected(object sender, XmppDisconnectedEventArgs e)
  {
    Console.WriteLine("Disconnected");
  }

  private static void xmpp_OnMessageIn(object sender, XmppMessageInEventArgs e)
  {
    Console.WriteLine(e.From + " said: " + e.MessageText);
    messageReceived = true;
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