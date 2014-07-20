using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Interfaces
{
    public interface IFinanceRepository
    {
        int countTotalCharges();
        int countTotalCharges(DateTime start, DateTime end);
        int countResolvedCharges();
        int countResolvedCharges(DateTime start, DateTime end);
        int countUnresolvedCharges();
        decimal incomeToDate();
        decimal outstandingBalance();
        IEnumerable<Charge> getUnresolvedCharges();
        IEnumerable<Charge> getUnresolvedCharges(int count, int skip);
        IEnumerable<Charge> getAllCharges();
        IEnumerable<Charge> getAllCharges(int count, int skip);
        void addCharge(Charge charge);
        void addCharge(decimal amount, string userName, string title, string description);
        void updateCharge(Charge charge);
        void updateCharge(int chargeId, decimal amountCharged, string title, string description);
        void closeCharge(int chargeId, decimal amountPaid);
    }
}
