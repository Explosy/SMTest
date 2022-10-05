using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTest.ViewModels
{
    internal class WareHouseAddViewModel : BindableBase
    {
        /// <summary>
        /// Свойство, отражающее веденный пользователем номер первого пикета на складе
        /// </summary>
        private int firstPicketNumber = 0;
        public int FirstPicketNumber
        {
            get => firstPicketNumber;
            set
            {
                SetProperty(ref firstPicketNumber, value);
            }
        }
        /// <summary>
        /// Свойство, отражающее веденное пользователем количество пикетов на складе
        /// </summary>
        private int picketsCount = 0;
        public int PicketsCount
        {
            get => picketsCount;
            set
            {
                SetProperty(ref picketsCount, value);
            }
        }

    }
}
