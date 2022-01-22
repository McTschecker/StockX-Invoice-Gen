# StockX Lexoffice Invoice Generator

This Project uses the StockX Sales export(CSV file) to create the corresponding invoices in Lexoffice using their API.

Please feel free to open an Issue for bugs

[Download app](https://github.com/McTschecker/StockX-Invoice-Gen/releases/latest)


[.net Runtime 5.0 (required to run)](https://dotnet.microsoft.com/download/dotnet/5.0/runtime?utm_source=getdotnetcore&utm_medium=referral) please download the Run console apps version

[Support Discord Server](https://discord.gg/mZJWvUkPva)

## Setup Guide
Please follow these steps to set up this program for the first time.

### Lexoffice API
1. [Login](https://app.lexoffice.de/home?cid=lxlp) to your Lexoffice Account
2. Now click the settings icon and go to extensions or click [here](https://app.lexoffice.de/settings/#/addons)
3. Scroll down all the way to more apps 
![Lexoffice Public API](/images/api.png)
4. Click the settings icon
5. Click Create new key (Schlüssel neu Erstellen in German)
![Lexoffice Public API](/images/apiKeyCreated.png)
6. Copy the api key and save it securely. You can not see it again after closing the dialog. (Only overwrite it)

### Configure the generator
For more info on the settings reference the [Settings file section](#settings)

1. Now [download](https://github.com/McTschecker/StockX-Invoice-Gen/releases/latest) the latest verison of the generator and unzip the content.
2. Open the `settings.json` file add the API key in the `LexofficeApiKey` field.

Now we need to find the unique ID of your StockX customer. 

3. Go to [your customer list](https://app.lexoffice.de/customer/#/list)
4. Find your stockX customer account and enter the details page
4. If you haven't already added the VAT ID now is a good time to do that. If not added, the generation will fail.
5. You will have an url like this `https://app.lexoffice.de/customer/#/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/turnover`. Now copy the id of the customer ( the content which looks like this: `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX`) and add it in the settings.json into the `contactID` in the `lexOfficeAdress` key. 
-> Otherwise generating the Reverse charge invoice will fail

6. Now finish filling out the settings of the adress of your customer.

# Running the application

1. Get the export of your sales from StockX [here](https://stockx.com/selling).
2. Wait for the email and then download the csv.
3. Copy the csv to the folder with the program.
4. Drag the csv onto the icon of the program. 
5. A new terminal should open and the export should automatically begin.
6. Check out the invoice on your lexoffice account


# Settings

## Settings section
`LexofficeApiKey`: The lexoffice API Key you want to export to 

`Finalize`: Finished the Invoice in Lexoffice. If false, then the invoice is saved as a draft. If true then the invoice is finished and **permanently** created and can only be canceled.

`DryRun`: Prevents the export of the invoice. This will cause the program to skip creation and skip adding the newly created invoices to the exportedCSV.

## lexOfficeAdress

This section includes the adress information of whom the program creates the invoice for. 

`name`: name of the entity on the invoice

`supplement`: supplemental text for the adress (adress line 2)

`street`: street of adress

`city`: city of Adress

`zip`: zip of adress  

`countryCode`: ISO country code of the entity on the invoice. **MUST BE AN EU COUNTRY** since this creates a **reverse charge** invoice.

`contactID`: The unique identifier of the customer you are creating the invoice for. This needs to be filled out, since lexoffice requires it for reverse charge.


# Legal Notice

There is no warranty for the correctness of the invoices. You are using this software as is and take all responsibility for any actions performed by it. Please verify yourself, that any actions performed by this are authorized by you and that you have the right to perform them.
