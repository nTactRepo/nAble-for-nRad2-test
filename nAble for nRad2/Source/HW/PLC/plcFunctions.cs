using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace nTact.PLC
{
    public class plcFunctions
    {
        /// <summary>ConvertToDecimal is a method to convert a 16 or 32 bit binary number to a long data type.
        /// </summary>
        public static int ConvertToDecimal(bool is32Bit, string bitData)
        {
            if (is32Bit)
            {
                bitData=bitData.PadLeft(32, (char)'0');
            }
            else
            {
                bitData=bitData.PadLeft(16,(char)'0');
            }
            int result = 0;
            bool[] boolBits = new bool[bitData.Length];
            //Convert string to bool data
            for (int i = bitData.Length; i > 0; i--)
            {
                boolBits[i-1] = false;
                if(bitData.Substring(i-1,1)=="1")
                {
                    boolBits[i-1] = true;
                }
            }
            Array.Reverse(boolBits);
            if (is32Bit)
            {
                int binValue = 0;
                if (boolBits[31]) //2's Compliment for Negative Number
                {
                    binValue = 1;
                    for (int i = 0; i < 32; i++)
                    {
                        if (!boolBits[i]) result = result + binValue;
                        binValue = binValue * 2;
                    }
                    result = (result+1) * -1;
                }
                else //Normal Processing for Positive Number
                {
                    binValue=1;
                    for (int i = 0; i < 32; i++)
                    {
                        if(boolBits[i]) result=result+binValue;
                        binValue = binValue * 2;
                    }
                }
            }
            else //16-bit Conversion
            {
                int binValue = 0;
                if (boolBits[15]) //2's Compliment for Negative Number
                {
                    binValue = 1;
                    for (int i = 0; i < 16; i++)
                    {
                        if (!boolBits[i]) result = result + binValue;
                        binValue = binValue * 2;
                    }
                    result = (result+1) * -1;
                }
                else //Normal Processing for Positive Number
                {
                    binValue=1;
                    for (int i = 0; i < 16; i++)
                    {
                        if(boolBits[i]) result=result+binValue;
                        binValue = binValue * 2;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Convert integer value to string of 2 ascii characters.
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public static string ConvertToAscii(int nData)
        {
            string strRetVal = "";
            string strTemp = ConvertToBinaryString(false, nData);
            strRetVal = new string(Convert.ToChar(Convert.ToInt32(strTemp.Substring(8, 8), 2)), 1);
            strRetVal = strRetVal +  new string(Convert.ToChar(Convert.ToInt32(strTemp.Substring(0, 8), 2)), 1);
        
            return strRetVal;
        }

        /// <summary>ConvertToBinaryString is a method to convert a 16 or 32 bit number to a binary string.
        /// </summary>
        public static string ConvertToBinaryString(bool is32Bit, int numberToConvert)
        {
            string binValue;
            string resultBin = "";
            int convValue = 0;
            if (numberToConvert >= 0)
            {
                resultBin = Convert.ToString(numberToConvert, 2).PadLeft((is32Bit ? 32:16),(char)'0');
            }
            else
            {
                convValue = Math.Abs(numberToConvert) - 1;
                binValue = Convert.ToString(convValue, 2);
                if (is32Bit)
                {
                    binValue = binValue.PadLeft(31, (char)'0');
                }
                else
                {
                    binValue = binValue.PadLeft(15, (char)'0');
                }

                for (int i = 0; i < binValue.Length; i++)
                {
                    resultBin = resultBin + (binValue.Substring(i, 1) == "0" ? "1" : "0");
                }
                resultBin = "1" + resultBin;
            }
            return resultBin;

        }

        /// <summary>ConvertToBinary is a method to convert a 16 or 32 bit number to an array of boolean values.
        /// </summary>
        public static bool[] ConvertToBinary(bool is32Bit, int numberToConvert)
        {
            bool[] bBinary;

            if (is32Bit)
            {
                bBinary = new bool[32];
            }
            else
            {
                bBinary = new bool[16];
                for (int i = 15; i > -1; i--)
                {
                    if (numberToConvert >= GlobalData.arrBinValue[i])
                    {
                        bBinary[i] = true;
                        numberToConvert = numberToConvert - GlobalData.arrBinValue[i];
                    }
                    else
                    {
                        bBinary[i] = false;
                    }
                }
            }
            return bBinary;

        }

        /// <summary>ConvertLongToIntegers is a method to convert a Int32 to two shorts.
        /// </summary>
        public static short[] ConvertLongToIntegers(int numberToConvert)
        {
            List<short> results = new List<short>();
            string strBinary = ConvertToBinaryString(true, numberToConvert);
            results.Add((short)ConvertToDecimal(false, strBinary.Substring(16, 16)));
            results.Add((short)ConvertToDecimal(false, strBinary.Substring(0, 16)));
            return results.ToArray();
        }

        /// <summary>
        /// Perform 2's complement conversion on a single word from startAddress.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        public static int TwosComp(string startAddress)
        {
            int iResult = 0;
            int[] lData = GetData(1, startAddress);
            string sData = ConvertToBinaryString(false, lData[0]);
            iResult = ConvertToDecimal(false, sData);
            return iResult;
        }

        /// <summary>
        /// Perform 2's complement conversion on a double word beginning at the start address.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        public static int TwosCompD(string startAddress)
        {
            int iResult = 0;
            int[] lData = GetData(2, startAddress);
            string sData = ConvertToBinaryString(false, lData[1]);
            sData = sData + ConvertToBinaryString(false, lData[0]);
            iResult = ConvertToDecimal(true, sData);
            return iResult;
        }

        /// <summary>
        /// Return the state of the specifice address.
        /// </summary>
        /// <param name="plcAddress"></param>
        /// <returns></returns>
        public static bool GetState(string plcAddress)
        {
            if (plcAddress == null || plcAddress == "")
                return false;

            string dataType = plcAddress.Substring(0, 1);
            plcAddress=plcAddress.Remove(0, 1);
            switch (dataType)
            {
                case "X":
                {
                    int Address = Convert.ToInt32(plcAddress, 8); //For Octal Conversion
                    //int Address = Convert.ToInt16(plcAddress.Remove(0, 1), 16); //For Hex Conversion
                    return GlobalData.Xbit[Address];
                }
                case "Y":
                {
                    int Address = Convert.ToInt32(plcAddress, 8); //For Octal Conversion
                    //int Address = Convert.ToInt16(plcAddress.Remove(0, 1), 16); //For Hex Conversion
                    return GlobalData.Ybit[Address];
                }
                case "M":
                {
                    int Address = Convert.ToInt16(plcAddress);
                    return GlobalData.Mbit[Address];
                }
                case "B":
                {
                    int Address = Convert.ToInt16(plcAddress.Remove(0, 1),16);
                    return GlobalData.Bbit[Address];
                }
                case "L":
                    {
                        int Address = Convert.ToInt16(plcAddress);
                        return GlobalData.Lbit[Address];
                    }
            }
            return false;
        }


        /// <summary>
        /// Return the word specified by sAddress.
        /// </summary>
        /// <param name="iSize"></param>
        /// <param name="sAddress"></param>
        /// <returns></returns>
        public static int GetWord(string sAddress)
        {
            int nRetVal = 0;
            int nAddress = 0;
            
            if (sAddress == null || sAddress == "")
                return 0;
            
            switch (sAddress.Substring(0, 1))
            {
                case "D":
                nAddress = Convert.ToInt16(sAddress.Remove(0, 1));
                nRetVal = GlobalData.Dword[nAddress];
                break;

                case "W":
                nAddress = Convert.ToInt16(sAddress.Remove(0, 1),16);
                nRetVal = GlobalData.Wword[nAddress];
                break;

                case "SD":
                 nRetVal = GlobalData.SDword[nAddress];
                break;
            }
            return nRetVal;
        }

        /// <summary>
        /// Return the number of words specified beginning at the startAddress.
        /// </summary>
        /// <param name="iSize"></param>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        private static int[] GetData(int iSize, string startAddress)
        {
            List<int> results = new List<int>();
            int Address = Convert.ToInt16(startAddress.Remove(0, 1));
            switch(startAddress.Substring(0,1))
            {
                case "D":
                    results.Add(GlobalData.Dword[Address]);
                    if (iSize > 1) results.Add(GlobalData.Dword[Address + 1]);
                    break;

                case "W":
                    results.Add(GlobalData.Wword[Address]);
                    if (iSize > 1) results.Add(GlobalData.Wword[Address + 1]);
                    break;

                case "SD":
                    results.Add(GlobalData.SDword[Address]);
                    if (iSize > 1) results.Add(GlobalData.SDword[Address + 1]);
                    break;
            }
            return results.ToArray();
        }

        /// <summary>
        /// Populate the array used when converting to binary.
        /// </summary>
        public static void PopulateBinaryValuesArray()
        {
            GlobalData.arrBinValue[0] = 1;
            for (int i = 1; i < 16; i++)
            {
                GlobalData.arrBinValue[i] = GlobalData.arrBinValue[i - 1] * 2;
            }
        }
    }
}
