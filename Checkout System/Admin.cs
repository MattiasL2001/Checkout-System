﻿
namespace Checkout_System
{
    static class Admin
    {
        public static void AddProduct()
        {
            string input;
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);
            Product product = new Product(0, 0, 0, "");

            while (true)
            {
                Console.WriteLine("Input an ID for the product");
                input = Console.ReadLine();

                if (!int.TryParse(input, out int intInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not recognize input as an integer!");
                    Console.ResetColor();
                    continue;
                }

                if (CheckIfProductExists(Convert.ToInt32(input)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("A product with the given ID already exists!");
                    Console.ResetColor();
                    continue;
                }

                product.ID = Convert.ToInt32(input);
                break;
            }

            while (true)
            {
                Console.WriteLine("Input a name for the product");
                input = Console.ReadLine();
                if (CheckIfProductExists(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("A product with the given name already exists!");
                    Console.ResetColor();
                }
                else { product.Name = input; break; }
            }

            while (true)
            {
                Console.WriteLine("Price per unit or price per KG?");
                Console.WriteLine("1. For price per unit");
                Console.WriteLine("2. For price per KG");
                input = Console.ReadLine().Trim();

                if (input == "1") { product.PriceType = Product.PriceTypes.PricePerUnit; break; }
                else if (input == "2")
                {
                    product.PriceType = Product.PriceTypes.PricePerKG;

                    while (true)
                    {
                        Console.WriteLine("Input weight per each item in KG!");
                        string answer = Console.ReadLine();

                        if (decimal.TryParse(answer, out decimal decimalAnswer))
                        {
                            Console.WriteLine("Product weight set!");
                            product.Weight = decimalAnswer;
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Could not regognize input as a float!");
                            Console.ResetColor();
                        }
                    }

                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input!");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.WriteLine("Input a price for the product");
                input = Console.ReadLine();

                if (int.TryParse(input, out int output)) { product.Price = output; break; }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not recognize input as an integer!");
                    Console.ResetColor();
                }
            }

            products = FileAndFormat.FileToProducts(App.productsFilePath);
            products.Add(product);
            FileAndFormat.ProductsToFile(products, App.productsFilePath);
        }

        public static object GetProduct(int id)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);

            if (CheckIfProductExists(id))
            {
                foreach (Product product in products)
                {
                    if (product.ID == id) { return product; }
                }
            }
            return null;
        }

        public static object GetProduct(string name)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);

            if (CheckIfProductExists(name))
            {
                foreach (Product product in products)
                {
                    if (product.Name == name) { return product; }
                }
            }
            return null;
        }

        public static void RemoveProduct(int id)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);

            if (CheckIfProductExists(id))
            {
                List<Product> newProductList = new List<Product>();

                products.ForEach(product =>
                {
                    if (product.ID == id)
                    {
                        Console.WriteLine("Successfully removed the product: " + product.Name);
                        newProductList.Remove(product);
                    }
                    else { newProductList.Add(product); }
                });
                products = newProductList;
                FileAndFormat.ProductsToFile(products, App.productsFilePath);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No product with the given ID exists!");
                Console.ResetColor();
            }
        }

        public static void RemoveProduct(string name)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);

            if (CheckIfProductExists(name))
            {
                List<Product> newProductList = new List<Product>();

                products.ForEach(product =>
                {
                    if (product.Name == name)
                    {
                        Console.WriteLine("Successfully removed the product: " + product.Name);
                        newProductList.Remove(product);
                    }
                    else { newProductList.Add(product); }
                });
                products = newProductList;
                FileAndFormat.ProductsToFile(products, App.productsFilePath);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No product with the given name exists!");
                Console.ResetColor();
            }
        }

        public static void ChangeProductPrice(int id)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);
            if (products.Count < id)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: ID is out of bounds");
                Console.ResetColor();
            }
        }

        public static void ChangeProductName(string name)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);

            if (CheckIfProductExists(name))
            {
                Console.WriteLine("Input new name for the product!");
                string answer = Console.ReadLine();

                if (!CheckIfProductExists(answer))
                {
                    products.ForEach(product =>
                    {
                        if (product.Name == name)
                        {
                            product.Name = answer;
                            Console.WriteLine("Name of product changed!");
                        }
                    });
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No products with the given name exists!");
                    Console.ResetColor();
                }
            }
        }

        public static bool CheckIfProductExists(int id)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);

            foreach (Product product in products)
            {
                if (product.ID == id) { return true; }
            }
            return false;
        }
        public static bool CheckIfProductExists(string name)
        {
            List<Product> products = FileAndFormat.FileToProducts(App.productsFilePath);
            foreach (Product product in products)
            {
                if (product.Name == name) { return true; }
            }
            return false;
        }
    }
}
