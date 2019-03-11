#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
public class Item : TableEntity
{
public string ItemName {get;set;}
}