namespace DemoWebAPIForSentry.Exceptions
{
    [Serializable]
    public class BreadcrumpException : Exception
    {
        public BreadcrumpException() { }

        public BreadcrumpException(string name)
            : base(string.Format("Creadcrump Exception"))
        {

        }
    }
}
