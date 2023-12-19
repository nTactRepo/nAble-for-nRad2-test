using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTact.Recipes
{
    public class DynamicDispenseProfileParamList : ArrayList
    {
        public DynamicDispenseProfileParamList() : base() { }
        public DynamicDispenseProfileParamList(int capacity) : base(capacity) { }
        public virtual int Add(DynamicDispenseProfileParam value)
        {
            return base.Add(value);
        }
        public virtual DynamicDispenseProfileParam Item(int Index)
        {
            return (DynamicDispenseProfileParam)base[Index];
        }

        public bool Get(int nAddress, ref DynamicDispenseProfileParam dispenseProfileParam)
        {
            bool bRetVal = false;

            if (Count > 0)
            {
                foreach (DynamicDispenseProfileParam curParam in this)
                {
                    if (curParam.ArrayLocation == nAddress)
                    {
                        bRetVal = true;
                        dispenseProfileParam = curParam;
                        break;
                    }
                }
            }
            return bRetVal;
        }
        public bool Update(DynamicDispenseProfileParam dispenseProfileParam)
        {
            bool bRetVal = false;

            if (Count > 0)
            {
                foreach (DynamicDispenseProfileParam curParam in this)
                {
                    if (curParam.ArrayLocation == dispenseProfileParam.ArrayLocation)
                    {
                        bRetVal = true;
                        curParam.Update(dispenseProfileParam);
                        break;
                    }
                }
                if (!bRetVal)
                {
                    Add(dispenseProfileParam);
                    bRetVal = true;
                }
            }
            else
            {
                Add(dispenseProfileParam);
                bRetVal = true;
            }
            return bRetVal;
        }

        public bool IsChanged(DynamicDispenseProfileParamList testList)
        {
            bool bRetVal = false;
            bRetVal = this.Count != testList.Count;
            if (!bRetVal)
            {
                foreach (DynamicDispenseProfileParam param in this)
                {
                    bRetVal |= param.IsChanged((DynamicDispenseProfileParam)testList[param.ArrayLocation]);
                }
            }

            return bRetVal;
        }
    }
}
