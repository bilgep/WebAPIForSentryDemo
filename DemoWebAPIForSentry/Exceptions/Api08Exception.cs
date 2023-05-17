namespace DemoWebAPIForSentry.Exceptions
{
    [Serializable]
    public class Api08Exception : Exception
    {
        public Api08Exception() { }

        public Api08Exception(string name)
            : base(string.Format("Api08 Exception"))
        {

        }
    }
}
