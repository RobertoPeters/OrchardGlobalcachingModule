using Globalcaching.Core;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Globalcaching.Models;
using Globalcaching.ViewModels;
using NopCommerce.Api.AdapterLibrary;
using System.Text;

namespace Globalcaching.Services
{
    public interface IShopService : IDependency
    {
        string Authorize(string redirectUrl);
        void GetAccessToken(string code, string state, string redirectUrl);
        void RefreshAccessToken();
        ShopAdminModel GetShopAdminModel();
        void SetMasterCategoryId(int id);
        ShopUserProductModel GetShopUserProductModel(int yafUserId);
        ShopUserProductModel GetUserProduct(int yafUserId, int? accessProductId);
        ShopUserProductModel AddUserProduct(int yafUserId, string name, int categoryId, string shortDescription, string fullDescription, double price);
        ShopUserProductModel DeleteUserProduct(int yafUserId, int productId);
        ShopUserProductModel SaveUserProduct(int yafUserId, int productId, string name, int categoryId, string shortDescription, string fullDescription, double price);
    }

    public class ShopService: IShopService
    {
        public const int MaxAllowedUserProducts = 10;

        public class AuthorizationModel
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("expires_in")]
            public long ExpiresIn { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

        public static string dbShopConnString = ConfigurationManager.ConnectionStrings["nopCommerceConnectionString"].ToString();

        private readonly IWorkContextAccessor _workContextAccessor;

        public ShopService(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        public string Authorize(string redirectUrl)
        {
            string result = null;

            var nopAuthorizationManager = new ShopAuthorizationManager(ConfigurationManager.AppSettings["shop_consumerkey"],
               ConfigurationManager.AppSettings["shop_consumersecret"],
               ConfigurationManager.AppSettings["shopBaseUrl"]);

            // This should not be saved anywhere.
            var state = Guid.NewGuid();
            HttpContext.Session["state"] = state;

            result = nopAuthorizationManager.BuildAuthUrl(redirectUrl, new string[] { }, state.ToString());

            return result;
        }


        private string FormatShortDescriptionForDatabase(string s)
        {
            var sb = new StringBuilder();
            sb.Append(HttpUtility.HtmlEncode(s??"")).Replace("\r", "").Replace("\n", "<br />");
            return sb.ToString();
        }

        private string FormatShortDescriptionForUser(string s)
        {
            return HttpUtility.HtmlDecode((s ?? "").Replace("<br />", "\r\n"));
        }

        private string FormatFullDescriptionForDatabase(string s)
        {
            var sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append(HttpUtility.HtmlEncode(s ?? "")).Replace("\r", "").Replace("\n", "<br />");
            sb.Append("</p>");
            return sb.ToString();
        }

        private string FormatFullDescriptionForUser(string s)
        {
            var orgText = s??"";
            var pos = s.IndexOf("<p>");
            if (pos >= 0)
            {
                var pos2 = s.IndexOf("</p>", pos + 1);
                if (pos2 > 0)
                {
                    orgText = s.Substring(pos + "<p>".Length, pos2 - pos - "<p>".Length);
                }
                else
                {
                    orgText = s.Substring(pos + "<p>".Length);
                }
            }
            return HttpUtility.HtmlDecode(orgText).Replace("<br />", "\r\n");
        }

        public void GetAccessToken(string code, string state, string redirectUrl)
        {
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
            {
                if (state != HttpContext.Session["state"].ToString())
                {
                    return;
                }

                try
                {
                    string clientId = ConfigurationManager.AppSettings["shop_consumerkey"];
                    string clientSecret = ConfigurationManager.AppSettings["shop_consumersecret"];
                    string serverUrl = ConfigurationManager.AppSettings["shopBaseUrl"];

                    var authParameters = new ShopAuthorizationManager.AuthParameters()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        ServerUrl = serverUrl,
                        RedirectUrl = redirectUrl,
                        GrantType = "authorization_code",
                        Code = code
                    };

                    var nopAuthorizationManager = new ShopAuthorizationManager(authParameters.ClientId, authParameters.ClientSecret, authParameters.ServerUrl);

                    string responseJson = nopAuthorizationManager.GetAuthorizationData(authParameters);

                    AuthorizationModel authorizationModel = JsonConvert.DeserializeObject<AuthorizationModel>(responseJson);

                    using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
                    {
                        var m = db.FirstOrDefault<ShopAccess>("");
                        if (m == null)
                        {
                            m = new ShopAccess();
                        }
                        m.ExpiresIn = authorizationModel.ExpiresIn;
                        m.RefreshToken = authorizationModel.RefreshToken;
                        m.Token = authorizationModel.AccessToken;
                        m.TokenType = authorizationModel.TokenType;
                        m.Updated = DateTime.Now;
                        db.Save(m);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void RefreshAccessToken()
        {
            try
            {
                string clientId = ConfigurationManager.AppSettings["shop_consumerkey"];
                string clientSecret = ConfigurationManager.AppSettings["shop_consumersecret"];
                string serverUrl = ConfigurationManager.AppSettings["shopBaseUrl"];

                ShopAccess m = null;
                using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
                {
                    m = db.FirstOrDefault<ShopAccess>("");
                }

                var authParameters = new ShopAuthorizationManager.AuthParameters()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    ServerUrl = serverUrl,
                    RefreshToken = m.RefreshToken,
                    GrantType = "refresh_token"
                };

                var nopAuthorizationManager = new ShopAuthorizationManager(authParameters.ClientId, authParameters.ClientSecret, authParameters.ServerUrl);
                string responseJson = nopAuthorizationManager.RefreshAuthorizationData(authParameters);
                AuthorizationModel authorizationModel = JsonConvert.DeserializeObject<AuthorizationModel>(responseJson);

                using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
                {
                    m.ExpiresIn = authorizationModel.ExpiresIn;
                    m.RefreshToken = authorizationModel.RefreshToken;
                    m.Token = authorizationModel.AccessToken;
                    m.TokenType = authorizationModel.TokenType;
                    m.Updated = DateTime.Now;
                    db.Save(m);
                }
            }
            catch
            {
            }
        }

        private string GetFullPathOfCategory(ShopCategory c, List<ShopCategory> allCategories)
        {
            string result = string.Empty;
            if (c != null)
            {
                if (c.ParentCategoryId>0)
                {
                    result = string.Format("{0} >> {1}", GetFullPathOfCategory((from a in allCategories where a.Id == c.ParentCategoryId select a).FirstOrDefault(), allCategories), c.Name);
                }
                else
                {
                    result = c.Name;
                }
            }
            else
            {
                result = "??";
            }
            return result;
        }

        public ShopAdminModel GetShopAdminModel()
        {
            var result = new ShopAdminModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
            {
                result.AllCategories = db.Fetch<ShopCategory>("where Deleted=0");
                foreach (var c in result.AllCategories)
                {
                    c.FullPath = GetFullPathOfCategory(c, result.AllCategories);
                }
                var m = db.FirstOrDefault<ShopAccess>("");
                if (m != null)
                {
                    result.TokenFromDate = m.Updated;
                    if (m.ExpiresIn.HasValue)
                    {
                        result.TokenExpires = m.Updated.AddSeconds(m.ExpiresIn.Value);
                    }
                    if (m.MasterCategoryId.HasValue)
                    {
                        result.MasterCategory = (from a in result.AllCategories where a.Id == m.MasterCategoryId.Value select a).FirstOrDefault();
                    }
                }
            }
            return result;
        }

        public void SetMasterCategoryId(int id)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
            {
                var m = db.FirstOrDefault<ShopAccess>("");
                if (m != null)
                {
                    m.MasterCategoryId = id;
                    db.Save(m);
                }
            }
        }

        public ShopUserProductModel GetShopUserProductModel(int yafUserId)
        {
            return GetUserProduct(yafUserId, null);
        }

        public ShopUserProductModel GetUserProduct(int yafUserId, int? accessProductId)
        {
            var result = new ShopUserProductModel();
            result.MaxAllowedProducts = MaxAllowedUserProducts;
            using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
            {
                result.AllCategories = db.Fetch<ShopCategory>("where Deleted=0");
                foreach (var c in result.AllCategories)
                {
                    c.FullPath = GetFullPathOfCategory(c, result.AllCategories);
                }
                result.Products = db.Fetch<ShopUserProductModelItem>("select GcEuProducts.*, Product.Name as Name, Product.Sku as Sku, ShortDescription = null, FullDescription = null, Price = 0, CategoryId=0  from GcEuProducts inner join Product on GcEuProducts.ProductId=Product.Id where GcEuProducts.UserId=@0 and Product.Published=1", yafUserId);
                result.ActiveProduct = db.FirstOrDefault<ShopUserProductModelItem>("select GcEuProducts.*, Product.Name as Name, Product.Sku as Sku, Product.ShortDescription as ShortDescription, Product.FullDescription as FullDescription, Product.Price as Price, CategoryId=0  from GcEuProducts inner join Product on GcEuProducts.ProductId=Product.Id where GcEuProducts.UserId=@0 and GcEuProducts.Id=@1", yafUserId, accessProductId);
                if (result.ActiveProduct != null)
                {
                    result.ActiveProduct.CategoryId = db.FirstOrDefault<int>("select CategoryId from Product_Category_Mapping where ProductId=@0", result.ActiveProduct.ProductId);
                    result.ActiveProduct.ShortDescription = FormatShortDescriptionForUser(result.ActiveProduct.ShortDescription);
                    result.ActiveProduct.FullDescription = FormatFullDescriptionForUser(result.ActiveProduct.FullDescription);
                }
            }
            return result;
        }

        public ShopUserProductModel DeleteUserProduct(int yafUserId, int productId)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
            {
                var userProduct = db.FirstOrDefault<GcEuProducts>("where Id=@0 and UserId=@1", productId, yafUserId);
                if (userProduct != null)
                {
                    var m = db.FirstOrDefault<ShopAccess>("");
                    if (m != null)
                    {
                        var nopApiClient = new ApiClient(m.Token, ConfigurationManager.AppSettings["shopBaseUrl"]);
                        string jsonUrl = string.Format("api/products/{0}", userProduct.ProductId);
                        var newProduct = new
                        {
                            product = new
                            {
                                published = false
                            }
                        };
                        string productJson = JsonConvert.SerializeObject(newProduct);

                        var addResult = nopApiClient.Put(jsonUrl, productJson) as string;
                    }
                }
            }
            return GetUserProduct(yafUserId, null);
        }

        public ShopUserProductModel SaveUserProduct(int yafUserId, int productId, string name, int categoryId, string shortDescription, string fullDescription, double price)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
            {
                var userProduct = db.FirstOrDefault<GcEuProducts>("where Id=@0 and UserId=@1", productId, yafUserId);
                if (userProduct != null)
                {
                    var m = db.FirstOrDefault<ShopAccess>("");
                    if (m != null)
                    {
                        var nopApiClient = new ApiClient(m.Token, ConfigurationManager.AppSettings["shopBaseUrl"]);
                        string jsonUrl = string.Format("api/products/{0}", userProduct.ProductId);
                        var newProduct = new
                        {
                            product = new
                            {
                                name = name,
                                short_description = FormatShortDescriptionForDatabase(shortDescription),
                                full_description = FormatFullDescriptionForDatabase(fullDescription),
                                price = price
                            }
                        };
                        string productJson = JsonConvert.SerializeObject(newProduct);
                        var addResult = nopApiClient.Put(jsonUrl, productJson) as string;

                        var mappingId = db.FirstOrDefault<int>("select Id from Product_Category_Mapping where ProductId=@0", productId);
                        if (mappingId > 0)
                        {
                            jsonUrl = string.Format("api/product_category_mappings/{0}", mappingId);
                            var ProductCategoryMapping = new
                            {
                                product_category_mapping = new
                                {
                                    product_id = productId,
                                    category_id = categoryId
                                }
                            };
                            string categoryMappingJson = JsonConvert.SerializeObject(ProductCategoryMapping);
                            addResult = nopApiClient.Put(jsonUrl, categoryMappingJson) as string;
                        }
                        else
                        {
                            jsonUrl = "api/product_category_mappings";
                            var ProductCategoryMapping = new
                            {
                                product_category_mapping = new
                                {
                                    product_id = productId,
                                    category_id = categoryId
                                }
                            };
                            string categoryMappingJson = JsonConvert.SerializeObject(ProductCategoryMapping);
                            addResult = nopApiClient.Post(jsonUrl, categoryMappingJson) as string;
                        }
                    }
                }
            }
            return GetUserProduct(yafUserId, productId);
        }

        public ShopUserProductModel AddUserProduct(int yafUserId, string name, int categoryId, string shortDescription, string fullDescription, double price)
        {
            int? newProductId = null;
            int? usrProductId = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbShopConnString, "System.Data.SqlClient"))
            {
                var m = db.FirstOrDefault<ShopAccess>("");
                if (m != null)
                {
                    var allUserProducts = db.Fetch<GcEuProducts>("where UserId=@0", yafUserId);
                    var deletedUserProducts = db.Fetch<GcEuProducts>("select GcEuProducts.* from GcEuProducts inner join Product on GcEuProducts.ProductId=Product.Id where GcEuProducts.UserId=@0 and Product.Published=0", yafUserId);
                    if (deletedUserProducts.Count > 0)
                    {
                        newProductId = deletedUserProducts[0].ProductId;
                        usrProductId = deletedUserProducts[0].Id;
                        var nopApiClient = new ApiClient(m.Token, ConfigurationManager.AppSettings["shopBaseUrl"]);
                        string jsonUrl = string.Format("api/products/{0}", newProductId);
                        var newProduct = new
                        {
                            product = new
                            {
                                name = name,
                                short_description = FormatShortDescriptionForDatabase(shortDescription),
                                full_description = FormatFullDescriptionForDatabase(fullDescription),
                                price = price,
                                published = true
                            }
                        };
                        string productJson = JsonConvert.SerializeObject(newProduct);

                        var addResult = nopApiClient.Put(jsonUrl, productJson) as string;
                        if (addResult != null)
                        {
                            var mappingId = db.FirstOrDefault<int>("select Id from Product_Category_Mapping where ProductId=@0", newProductId);

                            if (mappingId > 0)
                            {
                                jsonUrl = string.Format("api/product_category_mappings/{0}", mappingId);
                                var ProductCategoryMapping = new
                                {
                                    product_category_mapping = new
                                    {
                                        product_id = newProductId,
                                        category_id = categoryId
                                    }
                                };
                                string categoryMappingJson = JsonConvert.SerializeObject(ProductCategoryMapping);
                                addResult = nopApiClient.Put(jsonUrl, categoryMappingJson) as string;
                            }
                            else
                            {
                                jsonUrl = "api/product_category_mappings";
                                var ProductCategoryMapping = new
                                {
                                    product_category_mapping = new
                                    {
                                        product_id = newProductId,
                                        category_id = categoryId
                                    }
                                };
                                string categoryMappingJson = JsonConvert.SerializeObject(ProductCategoryMapping);
                                addResult = nopApiClient.Post(jsonUrl, categoryMappingJson) as string;
                            }
                        }
                    }
                    else if (allUserProducts.Count < MaxAllowedUserProducts)
                    {
                        var nopApiClient = new ApiClient(m.Token, ConfigurationManager.AppSettings["shopBaseUrl"]);
                        string jsonUrl = "api/products";
                        var newProduct = new
                        {
                            product = new
                            {
                                name = name,
                                sku = string.Format("usr-{0}-{1}", yafUserId, allUserProducts.Count+1),
                                short_description = FormatShortDescriptionForDatabase(shortDescription),
                                full_description = FormatFullDescriptionForDatabase(fullDescription),
                                price = price,
                                disable_buy_button = true,
                                disable_wishlist_button = true,
                                allow_customer_reviews = false
                            }
                        };
                        string productJson = JsonConvert.SerializeObject(newProduct);

                        var addResult = nopApiClient.Post(jsonUrl, productJson) as string;
                        if (addResult != null)
                        {
                            dynamic product = JsonConvert.DeserializeObject(addResult);
                            newProductId = Convert.ToInt32(product.products[0].id);

                            jsonUrl = "api/product_category_mappings";
                            var ProductCategoryMapping = new
                            {
                                product_category_mapping = new
                                {
                                    product_id = newProductId,
                                    category_id = categoryId
                                }
                            };

                            var userProduct = new GcEuProducts();
                            userProduct.ProductId = newProductId.Value;
                            userProduct.UserId = yafUserId;
                            userProduct.ProductCode = Guid.NewGuid().ToString("N");
                            db.Save(userProduct);
                            usrProductId = userProduct.Id;

                            string categoryMappingJson = JsonConvert.SerializeObject(ProductCategoryMapping);

                            addResult = nopApiClient.Post(jsonUrl, categoryMappingJson) as string;
                            if (addResult != null)
                            {
                            }
                        }
                    }
                    else
                    {
                        //maximum reached, so not allowed
                    }
                }
            }
            return GetUserProduct(yafUserId, usrProductId);
        }

    }
}