namespace BMT_backend.Models
{
    public class ProductModel
    {
        // Attributes filled in first post
        public string Id { get; set; } = null!; // auto generated
        public string Username { get; set; }    // from localstorage
        public string Name { get; set; } // form
        public string Description { get; set; } // form
        public double Weight { get; set; }  // form
        public double Price { get; set; }   //form
        public bool? Type { get; set; }  // form

        // Atributes filled in second post
        public List<string>? Tags { get; set; } // 
        public List<string>? ImagesURLs { get; set; }
    }
        public class NonPerishableProductModel
    {
        public string ProductId { get; set; } = null!;  // first post response
        public int Stock { get; set; } // form
    }

    public class PerishableProductModel
    {
        public string ProductId { get; set; } = null!;  // first post response
        public int Stock { get; set; }  // form
        public int WeekDaysAvailable { get; set; }  // form
        public int DayLimitAvailable { get; set; }  // form
    }
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public bool Type { get; set; }
        public string EnterpriseName { get; set; }
        public List<string> Tags { get; set; }
        public List<string> ImagesURLs { get; set; }
    }

    public class AddTagsToProductRequest {
        public string ProductId { get; set; }
        public List<string> Tags { get; set; }
    }
}

