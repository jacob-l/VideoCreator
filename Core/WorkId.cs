using System;

namespace Core
{
    [Serializable]
    public class WorkId
    {
        int number;
        int fullNumber;
        int startCapture;
        int endCapture;

        public WorkId(int number, int fullNumber, int startCapture, int endCapture)
        {
            if (number < 0 || number >= fullNumber)
                throw new ArgumentException("Number should be between 0 and fuulNumber-1");
            this.number = number;
            this.fullNumber = fullNumber;
            this.startCapture = startCapture;
            this.endCapture = endCapture;
        }

        public int Number
        {
            get
            {
                return number;
            }
        }

        public int FullNumber
        {
            get
            {
                return fullNumber;
            }
        }

        public int StartCapture
        {
            get
            {
                return startCapture;
            }
        }

        public int EndCapture
        {
            get
            {
                return endCapture;
            }
        }
    }
}
