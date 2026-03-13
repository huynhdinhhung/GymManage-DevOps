namespace GYM_Manage.Models
{
    public class OtpCode
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool Used { get; set; } = false;
    }

}
