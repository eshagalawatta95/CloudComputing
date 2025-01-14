﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BudgetMe.Core.Views.UserControls;
using BudgetMe.Enums;
using BudgetMe.Entities;
using BudgetMe.Core.Service;
using System.Drawing;

namespace BudgetMe.Views.UserControls.Report
{
    public partial class ReportUserControl : UserControl, ITransactionUserControl
    {
        private IApplicationService _applicationService;
        private BindingList<TransactionBinder> _transactionBinders;
        private BindingList<ScheduleTransactionBinder> _scheduleTransactionBinder;

        public ReportUserControl()
        {
            _applicationService = BudgetMe.Entities.BudgetMeApplication.DependancyContainer.GetInstance<IApplicationService>();
            InitializeComponent();
            comboBoxType.DataSource = Enum.GetValues(typeof(ContentReportsTypesEnum));
            dateTimePickerForm.Value= new DateTime(DateTime.Now.Year, 1, 1); 
        }

        private void UpdateTransactionBinders(DateTime dtFrom, DateTime dtTo)
        {
            BindingList<TransactionBinder> transactionBinders = new BindingList<TransactionBinder>();

            IEnumerable<TransactionEntity> trans = _applicationService.Transactions.Where(x => x.TransactionDateTime >= dtFrom && x.TransactionDateTime <= dtTo && x.IsActive == true).OrderByDescending(t => t.TransactionDateTime);
            foreach (TransactionEntity transaction in trans)
            {
                if (transaction.IsActive)
                {
                    TransactionCategoryEntity transactionCategoryEntity = _applicationService.TransactionCategories.First(tp => tp.Id == transaction.TransactionCategoryId);
                    transactionBinders.Add(new TransactionBinder(transaction, transactionCategoryEntity));
                }
            }

            _transactionBinders = transactionBinders;
            dataGridView.DataSource = _transactionBinders;

          
        }

        private void UpdateScheduleTransactionBinders(DateTime dtFrom, DateTime dtTo)
        {
            BindingList<ScheduleTransactionBinder> transactionBinders = new BindingList<ScheduleTransactionBinder>();

            IEnumerable<SheduledTransactionList> trans = _applicationService.SheduledTransactions.Where(x => x.NextTransactionDate >= dtFrom && x.NextTransactionDate <= dtTo && x.IsActive == true).OrderByDescending(t => t.NextTransactionDate);
            foreach (SheduledTransactionList transaction in trans)
            {
                if (transaction.IsActive)
                {
                    TransactionCategoryEntity transactionCategoryEntity = _applicationService.TransactionCategories.First(tp => tp.Id == transaction.TransactionCategoryId);
                    transactionBinders.Add(new ScheduleTransactionBinder(transaction, transactionCategoryEntity));
                }
            }

            _scheduleTransactionBinder = transactionBinders;
            dataGridView.DataSource = _scheduleTransactionBinder;


        }

        private void contentHeaderUserControl_AddButtonOnClick(object sender, EventArgs e)
        {
            panelTools.Visible = true;
            tabControlReports.SelectedIndex = 0;
            dataGridView.DataSource = null;
        

        }
 
        private void btnOk_Click(object sender, EventArgs e)
        {
            panelTools.Visible = false;
         
            DateTime dtFrom = dateTimePickerForm.Value;
            DateTime dtTo = dateTimePickerTo.Value;
            int ReportVal = comboBoxType.SelectedIndex;

            if (ReportVal == 0)
            {
                UpdateTransactionBinders(dtFrom, dtTo);
            }

            else if (ReportVal == 1)
            {
                UpdateScheduleTransactionBinders(dtFrom, dtTo);
            }
  
            tabControlReports.Visible = true;
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dataGridView.Rows)
                if (Myrow.Cells[4].Value.ToString().Contains("-"))
                {
                    Myrow.Cells["Amount"].Style.ForeColor = Color.Red;
                }
                else
                {
                    Myrow.Cells["Amount"].Style.ForeColor = Color.Green;
                }
        }
    }

    class TransactionBinder
    {
        public TransactionBinder()
        { }

        public TransactionBinder(TransactionEntity transactionEntity, TransactionCategoryEntity transactionCategoryEntity)
        {
            ReferenceNumber = transactionEntity.ReferenceNumber;
            TransactionCategory = transactionCategoryEntity.Code;
            Amount = ((transactionEntity.IsIncome ? 1 : -1) * transactionEntity.Amount).ToString("0.00");
            IsScheduledTransaction = transactionEntity.ScheduledTransactionId == null ? "No" : "Yes";
            TransactionDateTime = transactionEntity.TransactionDateTime;
            Remarks = transactionEntity.Remarks;
            PerformedBy = transactionEntity.IsUserPerformed ? "User" : "System";
        }

        public string ReferenceNumber { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string TransactionCategory { get; set; }
        public string IsScheduledTransaction { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
        public string PerformedBy { get; set; }
    }

    class ScheduleTransactionBinder
    {
        public ScheduleTransactionBinder()
        { }

        public ScheduleTransactionBinder(SheduledTransactionList transactionEntity, TransactionCategoryEntity transactionCategoryEntity)
        {
            ReferenceNumber = transactionEntity.ReferenceNumber;
            TransactionCategory = transactionCategoryEntity.Code;
            Amount = ((transactionEntity.IsIncome ? 1 : -1) * transactionEntity.Amount).ToString("0.00");
            RepeatType = transactionEntity.RepeatType;
            NextTransactionDate = transactionEntity.NextTransactionDate;
            Remarks = transactionEntity.Remarks;
            EndTransactionDate = transactionEntity.EndDateTime.ToString();
        }

        public string ReferenceNumber { get; set; }
        public DateTime NextTransactionDate { get; set; }
        public string EndTransactionDate { get; set; }
        public string TransactionCategory { get; set; }
        public string RepeatType { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
    }
}
