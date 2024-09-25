using System.ComponentModel.DataAnnotations;

namespace SassProject.Models
{
    public enum State
    {
        بغداد,
        البصرة,
        نينوى,
        اربيل,
        كركوك,
        السليمانية,
        دهوك,
        الانبار,
        بابل,
        النجف,
        كربلاء,
        ميسان,
        ديالى,
        واسط,
        صلاح_الدين,
        المثنى,
        ذي_قار,
        الديوانية,
    }
    public enum OrderStatus
    {
        PENDING,
        APPROVED,
        DELIVERED,
        COMPLETED,
        CANCELLED
    }
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string NearestPoint { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        //public string CreatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        //public string UpdatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.PENDING;


        public double TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public State State { get; set; }


        public void CalculateTotalAmount()
        {
            TotalAmount = OrderItems.Sum(item => item.Total);
        }
    }
}
