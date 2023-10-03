(*
 * IPWorks IoT 2022 Delphi Edition - Sample Project
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
 *)

program coapserver;

uses
  Forms,
  coapserverf in 'coapserverf.pas' {FormCoapserver};

begin
  Application.Initialize;

  Application.CreateForm(TFormCoapserver, FormCoapserver);
  Application.Run;
end.


         
