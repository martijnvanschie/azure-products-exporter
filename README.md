# Azure Product Explorer README

## Goal

While looking for an Graph API which exposes a list of products and catagories i could not find one.

I searched and searched and ended up creating this tool that exports the list from the Azure product page by parsing the HTML and using the [Html Agility Pack](https://html-agility-pack.net/) to extract the categories and products.

-- You are welcome --

## Disclaimer

The code does not do much and i did not spend any time taking naming conventions, coding standards or best practices into account ;)

## Usage

Basically pull the code, compile (using .Net 7.0).

The just run the `ape.exe` command and wait for the command-line to finish. This could take up to 1 second so please be patient. (measured on my local machine, results van differ depending on your internet connection)

## Output

A new output file will be saved (or overwritten) in the root folder where the command-line is executed. The filename is **HARDCODED** (i know) and is called `azureproducts.json`. Make sure you do not have a file with the same name in the root when running for the first time.

The output file contains the following information.

```json
[
    {
        "id": "Auto generated category id",
        "name": "Category name as provided by Microsoft page",
        "skills": [
            {
                "id": "Auto generated category id",
                "name": "Skill name as provided by Microsoft page"
            }
        ]
    }
]
```

For convenience the file is also provided in the root of this repo.

## Resources

[Directory of Azure Cloud Services](https://azure.microsoft.com/en-us/products/)

[Html Agility Pack](https://html-agility-pack.net/) 