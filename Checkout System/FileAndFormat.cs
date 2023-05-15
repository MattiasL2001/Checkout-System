﻿
using System.Collections.Generic;

namespace Checkout_System
{
    internal class FileAndFormat
    {
        public static string ChangeFileDirectory()
        {
            while (true)
            {
                Console.WriteLine("Enter an existing directory!");
                Console.WriteLine("Input 'Back' to go to the main menu.");
                string directory = Console.ReadLine();
                string filePath = "";
                string fileName;

                if (directory.ToLower() == "back")
                {
                    return "back";
                }

                if (Directory.Exists(directory))
                {
                    Console.WriteLine("Enter a file name:");
                    fileName = Console.ReadLine();
                    try
                    {
                        File.WriteAllText(directory + "\\" + fileName + ".txt", "");
                        filePath = directory + "\\" + fileName + ".txt";
                        Console.WriteLine("Directory changed to:");
                        Console.WriteLine(filePath);
                        return "back";
                    }
                    catch
                    {
                        Console.WriteLine("Access denied to that directory, choose another one:");
                        Console.WriteLine(new UnauthorizedAccessException());
                    }
                }
                else
                {
                    Console.WriteLine("You must enter a VALID directory that exists!");
                }
            }
        }

        public static void ProductsToFile(List<Product> list, string productsFilePath)
        {
            string stringBuilder = "";

            for (int i = 0; i < list.Count; i++)
            {
                stringBuilder += "{\n   ";
                stringBuilder += list[i].ID + ",\n   ";
                stringBuilder += list[i].Price + ",\n   ";
                stringBuilder += list[i].Name + ",\n   ";
                stringBuilder += list[i].PriceType;

                if (list[i].PriceType == Product.PriceTypes.PricePerKG)
                {
                    stringBuilder += ",\n   ";
                    string stringWeight = list[i].Weight.ToString();
                    stringWeight = stringWeight.Replace(",", ".");
                    stringBuilder += stringWeight;
                }

                stringBuilder += "\n}";

                if (list.Count > 1 && i + 1 < list.Count) { stringBuilder += ",\n"; }
            }

            File.WriteAllText(productsFilePath, stringBuilder);
        }

        public static List<Product> FileToProducts(string filePath)
        {
            string fileContent = "";
            List<Product> products = new List<Product>();

            if (File.Exists(filePath)) { fileContent = File.ReadAllText(filePath); }
            else
            {
                File.WriteAllText(filePath, "");
                fileContent = File.ReadAllText(filePath);
            }

            if (fileContent.Contains("},"))
            {
                string[] productsString = fileContent.Split("},");

                foreach (string product in productsString)
                {
                    string prodString = product;
                    prodString = prodString.Replace("},", "");
                    prodString = prodString.Replace("{", "");
                    prodString = prodString.Replace("}", "");
                    prodString = prodString.Replace("\n", "");

                    int productId = Convert.ToInt32(prodString.Split(',')[0]);
                    int productPrice = Convert.ToInt32(prodString.Split(',')[1]);
                    string productName = prodString.Split(',')[2].Trim();
                    Product.PriceTypes productPriceType;

                    if (prodString.Split(",")[3].Contains("Unit"))
                    {
                        productPriceType = Product.PriceTypes.PricePerUnit;
                        products.Add(new Product(productId, productPrice, productPriceType, productName));
                    }
                    else
                    {
                        productPriceType = Product.PriceTypes.PricePerKG;
                        float weight = ConvertWeightToFloat(prodString.Split(",")[4]);
                        products.Add(new Product(productId, productPrice, productPriceType, productName, weight));
                    }
                }
            }
            else if (fileContent != "")
            {
                fileContent = fileContent.Replace("}", "");
                fileContent = fileContent.Replace("{", "");
                fileContent = fileContent.Replace("\n", "");
                int productID = Convert.ToInt32(fileContent.Split(',')[0]);
                int productPrice = Convert.ToInt32(fileContent.Split(',')[1]);
                string productName = fileContent.Split(",")[2].Trim();
                Product.PriceTypes productPriceType = new Product.PriceTypes();

                if (fileContent.Split(",")[3].Contains("Unit"))
                {
                    productPriceType = Product.PriceTypes.PricePerUnit;
                    products.Add(new Product(productID, productPrice, productPriceType, productName));
                }
                else
                {
                    productPriceType = Product.PriceTypes.PricePerKG;
                    float weight = ConvertWeightToFloat(fileContent.Split(",")[4]);
                    products.Add(new Product(productID, productPrice, productPriceType, productName, weight));
                }
            }

            fileContent = fileContent.Replace("},", "");
            fileContent = fileContent.Replace("}", "");
            fileContent = fileContent.Replace("{", "");
            fileContent = fileContent.Replace("\n", "");

            return products;
        }

        static float ConvertWeightToFloat(string weight)
        {
            if (weight.Contains(".")) { weight = weight.Replace(".", ","); }

            return Convert.ToSingle(weight);
        }

        public static void ClearProducts(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
            }
            else
            {
                Console.WriteLine("The file:");
                Console.WriteLine(filePath);
                Console.WriteLine("does not exist!");
            }
        }

        public static int GetPreviousSerialNumber(string receiptFilePath)
        {
            if (!File.Exists (receiptFilePath)) { File.WriteAllText(receiptFilePath, ""); }

            List<string> receipts = File.ReadAllText(receiptFilePath).
                Split("----------------------------------").ToList();
            List<string> receiptObject = new List<string>();
            string serialNumber = "0";
            receipts.ForEach(receipt =>
            {
                if (receipt != "" && receipt != "\n")
                {
                    if (receipt.Split("\n")[1].Contains("SerialNumber"))
                    {
                        serialNumber = receipt.Split("\n")[1].Replace("SerialNumber: ", "");
                    }
                    else
                    {
                        serialNumber = receipt.Split("\n")[2].Replace("SerialNumber: ", "");
                    }
                }
            });

            return Convert.ToInt32(serialNumber);
        }

        public static string ReceiptFileToString(Receipt receipt, string receiptFilePath)
        {
            double totalPrice = 0;
            string stringBuilder = "";
            int serialNumber = GetPreviousSerialNumber(receiptFilePath) + 1;
            stringBuilder += "RECEIPT: " + DateTime.Now + "\n";
            stringBuilder += $"SerialNumber: {serialNumber}\n";

            receipt.ProductList.ForEach(receiptObject =>
            {
                var product = receiptObject.Product;
                double price = product.Price;
                List<Campaign> campaignList = GetCampaignsForProduct(product.ID);

                campaignList.ForEach(campaign =>
                {
                    double discount = 0.01 * (100 - campaign.DiscountPercent);
                    stringBuilder += $"CAMPAIGN: {campaign.Title}, {campaign.DiscountPercent}% OFF\n";
                    stringBuilder += $"Price: {price} * {discount} = {price * discount}\n";
                    price *= discount;
                });

                if (receiptObject.Product.PriceType == Product.PriceTypes.PricePerKG)
                {
                    totalPrice += product.Price * product.Weight;
                    stringBuilder += $"{product.Name} {price}kr/kg x {product.Weight.ToString("0.0")}kg";
                    stringBuilder += $" = {totalPrice}kr\n";
                }
                else
                {
                    totalPrice += receiptObject.Quantity * price;
                    stringBuilder +=
                    $"{receiptObject.Product.Name} {receiptObject.Quantity} x {price}kr";
                    stringBuilder += $" = {totalPrice}kr\n";
                }
            });

            stringBuilder += $"Total: {totalPrice}kr\n";
            stringBuilder += "----------------------------------" + "\n";
            return stringBuilder;
        }
        public static void ReceiptToFile(Receipt receipt)
        {
            string receiptFilePath = "RECEIPT" + DateTime.Today.ToString("yyyyMMdd") + ".txt";

            string receiptString = ReceiptFileToString(receipt, receiptFilePath);
            File.WriteAllText(receiptFilePath, File.ReadAllText(receiptFilePath) + receiptString);
            receipt.ProductList.Clear();
        }

        public static List<Campaign> FileToCampaigns(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                return new List<Campaign>();
            }
            else
            {
                List<string> campaignsString;
                var campaignList = new List<Campaign>();

                if (File.ReadAllText(filePath).Contains("},\n{"))
                { campaignsString = File.ReadAllText(filePath).Trim().Split("},\n{").ToList(); }
                else { campaignsString = File.ReadAllText(filePath).Trim().Split("}").ToList(); }

                campaignsString.ForEach(campaign =>
                {
                    if (campaign != "")
                    {
                        string[] campaignElements = campaign.Split(",");

                        for (int i = 0; i < campaignElements.Length; i++)
                        {
                            campaignElements[i] = campaignElements[i].Trim();
                            campaignElements[i] = campaignElements[i].Replace("\n", "");
                            campaignElements[i] = campaignElements[i].Replace("}", "");
                            campaignElements[i] = campaignElements[i].Replace("{", "");
                        }

                        int id = Convert.ToInt32(campaignElements[0]);
                        int discount = Convert.ToInt32(campaignElements[1]);
                        string title = campaignElements[2];
                        Campaign c = new Campaign(id, discount, title);
                        campaignList.Add(c);
                    }
                });

                return campaignList;
            }
        }

        public static void CampaignsToFile(List<Campaign> campaignList, string filepath)
        {
            string stringBuilder = "";

            for (int i = 0; i < campaignList.Count; i++)
            {
                if (campaignList[i].Title == "") { campaignList[i].Title = "\"\""; }

                stringBuilder += "{\n   ";
                stringBuilder += campaignList[i].ID + ",\n   ";
                stringBuilder += campaignList[i].DiscountPercent + ",\n   ";
                stringBuilder += campaignList[i].Title + "\n";
                stringBuilder += "}";

                if (campaignList.Count > i && i + 1 < campaignList.Count) { stringBuilder += ",\n"; }
            }

            File.WriteAllText(filepath, stringBuilder);
        }

        public static List<Campaign> GetCampaignsForProduct(int productID)
        {
            Product product = new Product(0, 0, Product.PriceTypes.PricePerKG, "");
            bool productExists = false;

            FileToProducts(App.productsFilePath).ForEach(prod =>
            {
                if (prod.ID == productID)
                {
                    product = prod;
                    productExists = true;
                }
            });

            if (!productExists) { return new List<Campaign>(); }

            List<Campaign> campaignList = FileToCampaigns(App.campaignsFilePath);
            var productCampaigns = new List<Campaign>();

            campaignList.ForEach(campaign =>
            {
                if (campaign.ID == productID)
                {
                    productCampaigns.Add(campaign);
                }
            });

            return productCampaigns;
        }
    }
}
