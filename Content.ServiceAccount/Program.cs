using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Content.ServiceAccount
{
    public class Program
    {
        // MERCHANT ID
        private static ulong MERCHANT_ID = 0;

        // SERVICE ACCOUNT EMAIL ADDRESS
        private static string SERVICE_EMAIL_ADDRESS = "xxxxxxxxxxxxxxxxx@developer.gserviceaccount.com";

        // CERTIFICATE FILE LOCATION
        private static string CERT_LOCATION = @"c:\key.p12";

        public static void Main(string[] args)
        {
            Console.WriteLine("Content API - Service Account");
            Console.WriteLine("==========================");

            // Load certificate
            var certificate = new X509Certificate2(CERT_LOCATION, "notasecret", X509KeyStorageFlags.Exportable);

            // Create the credential
            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(SERVICE_EMAIL_ADDRESS)
               {
                   Scopes = new[] { ShoppingContentService.Scope.Content }
               }.FromCertificate(certificate));

            // Create the service.
            var service = new ShoppingContentService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Shopping Content Sample",
            });

            //========================================================================
            //Uncomment the methods you would like to run below
            //========================================================================

            ListProducts(service, 10);
            //GetProduct(service, "online:en:GB:21325023");
            //UpdateProduct(service, "online:en:GB:21325023");
            //DeleteProduct(service, "online:en:GB:21325023");
            //CreateProduct(service);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void GetProduct(ShoppingContentService service, string productId)
        {
            try
            {
                var Prod = service.Products.Get(MERCHANT_ID, productId).Execute();
                Console.WriteLine(Prod.Id + ": " + Prod.Title);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ListProducts(ShoppingContentService service, int amountToList)
        {
            try
            {
                var ProdList = service.Products.List(MERCHANT_ID).Execute();

                for (int i = 0; i < amountToList; i++)
                {
                    Console.WriteLine(ProdList.Resources[i].Id + ": " + ProdList.Resources[i].Title);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void UpdateProduct(ShoppingContentService service, string productId)
        {
            InventorySetRequest req = new InventorySetRequest();
            
            Price pri = new Price
                {
                    Currency = "GBP",
                    Value = "12.99",
                };

            req.Availability = "in stock";
            req.Price = pri;

            try
            {
                service.Inventory.Set(req, MERCHANT_ID, "online", productId).Execute();
                Console.WriteLine("Product " + productId + " updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void DeleteProduct(ShoppingContentService service, string productId)
        {
            Console.WriteLine("Are you sure you want to delete this product? It can't be undone.");
            var key = Console.ReadKey();

            if (key.KeyChar != 'Y' && key.KeyChar != 'y')
            {
                Console.WriteLine("Delete cancelled");
                return;
            }

            try
            {
                service.Products.Delete(MERCHANT_ID, productId).Execute();
                Console.WriteLine("Product " + productId + " deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void CreateProduct(ShoppingContentService service)
        {
            try
            {
                Product product = GenerateProduct();

                Product response = service.Products.Insert(product, MERCHANT_ID).Execute();
                Console.WriteLine(
                    "Product inserted with ID \"{0}\" and title \"{1}\".",
                    response.Id,
                    response.Title);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static Product GenerateProduct()
        {
            Product product = new Product();
            product.OfferId = "0123456789";
            product.Title = "A Tale of Two Cities";
            product.Description = "A classic novel about the French Revolution";
            product.Link = "http://my-book-shop.com/tale-of-two-cities.html";
            product.ImageLink = "http://my-book-shop.com/tale-of-two-cities.jpg";
            product.ContentLanguage = "EN";
            product.TargetCountry = "US";
            product.Channel = "online";
            product.Availability = "in stock";
            product.Condition = "new";
            product.GoogleProductCategory = "Media > Books";
            product.Gtin = "9780007350896";
            product.Price = new Price();
            product.Price.Currency = "USD";
            product.Price.Value = "2.50";

            ProductShipping shipping = new ProductShipping();
            shipping.Country = "US";
            shipping.Service = "Standard shipping";
            product.Shipping = new List<ProductShipping>();
            shipping.Price = new Price();
            shipping.Price.Currency = "USD";
            shipping.Price.Value = "0.99";
            product.Shipping.Add(shipping);

            product.ShippingWeight = new ProductShippingWeight();
            product.ShippingWeight.Unit = "grams";
            product.ShippingWeight.Value = 200;

            return product;
        }
    }
}
