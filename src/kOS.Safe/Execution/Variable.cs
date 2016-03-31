namespace kOS.Safe.Execution
{
    public class Variable
    {
        public string Name { get; set; }
        private object value;
        public virtual object Value
        {
            get { return value; }
            set
            {
                // Be very careful here to notice the difference
                // between _value and value:
                object oldValue = this.value;
                
                this.value = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
