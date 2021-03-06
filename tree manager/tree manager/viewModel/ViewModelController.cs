﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using DataCollectorLayer;
using SendManager;
using tree_manager.Properties;
using System.Linq;
using System.Threading;
namespace tree_manager.viewModel
{
    /**
     * klasa zarządzająca widokiem
     */
    public class ViewModelController : ObservableObject
    {

        public ViewModelController()
        {
            _currentDriver = "System.Data.SqlClient";
            _isExecuteButtonActive = true;
            _maxChunkSize = Settings.Default.maxSize;
            _connectionString = @Settings.Default.ConnectionString;//@"Server=MATEUSZ\SQLEXPRESS;Database=tree;Trusted_Connection=True;";
            var list = new List<string> { "SELECT * FROM [TreeList]" };

            CurrentIndex = "0-0";
            _recordsOnScreen = 20;
            _startFrom = 0;
            AmountOfRecordsInDatabase = 0;
            _oldRecordsOnScreen = RecordsOnScreen;
            BackupData = new FileDumper(_maxChunkSize);
        }

        public int AmountOfRecordsInDatabase { get; set; }
        public IDataProvider DataReader { get; set; }
        public IBackupData BackupData { get; set; }

        private ObservableCollection<DataGridColumn> _columns = new ObservableCollection<DataGridColumn>();
        private bool _isExecuteButtonActive;
        private readonly int _maxChunkSize;
        private int _recordsOnScreen;
        private int _startFrom;
        private int _oldRecordsOnScreen;
        private bool _isVirtualListEmpty;
        private VirtualizedPageGrid _virtualList;
        private string _currentQuery;
        private string _connectionString;
        private string _currentDriver;
        private bool _isNextButtonActive;
        private bool _isPreviousButtonActive;
        private string _indexBar;
        private BackgroundWorker _backgroundDataReader;
        private string _sName;
        private string _sIllName;
        private string _sSpecie;
        private int _fromAge;
        private int _toAge;

        string[] _virtualListSelectedRow = null;

        #region properties
        public string SelectedItemSpecie
        {
            get
            {
                try
                {
                    return _virtualListSelectedRow[0];
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
        public string SelectedItemName
        {
            get
            {
                try
                {
                    return _virtualListSelectedRow[1];
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
        public string SelectedItemAge
        {
            get
            {
                try
                {
                    return _virtualListSelectedRow[2];
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
        public string SelectedItemIllName
        {
            get
            {
                try
                {
                    return _virtualListSelectedRow[3];
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// zwraca wybrany rząd i podnosi "changed property"
        /// </summary>
        public string[] VirtualListSelectedRow
        {
            get { return _virtualListSelectedRow; }
            set
            {
                _virtualListSelectedRow = value;
                RaisePropertyChangedEvent("SelectedItemName");
                RaisePropertyChangedEvent("SelectedItemAge");
                RaisePropertyChangedEvent("SelectedItemIllName");
                RaisePropertyChangedEvent("SelectedItemSpecie");
            }
        }
        public string SSpecie
        {
            get { return _sSpecie; }
            set { _sSpecie = value; }
        }
        public int FromAge
        {
            get { return _fromAge; }
            set { _fromAge = value; }
        }
        public int ToAge
        {
            get { return _toAge; }
            set { _toAge = value; }
        }
        public string SName
        {
            get { return _sName; }
            set { _sName = value; }
        }
        public string SIllName
        {
            get { return _sIllName; }
            set { _sIllName = value; }
        }
        /// <summary>
        /// zwraca ilość rekordów zcatchowanych na dysku
        /// </summary>
        public int RecordsCached
        {
            get { return AmountOfRecordsInDatabase; }
            set
            {
                AmountOfRecordsInDatabase = value;
                if (!IsNextButtonActive)
                {
                    IsNextButtonActive = (_startFrom + _recordsOnScreen < AmountOfRecordsInDatabase);
                    SetIndex();
                }
                RaisePropertyChangedEvent("RecordsCached");
            }
        }

        /// <summary>
        /// lista kolumn
        /// </summary>
        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
                RaisePropertyChangedEvent("Columns");
            }
        }
       /// <summary>
       /// datagrid content
       /// </summary>
        public VirtualizedPageGrid VirtualList
        {
            get { return _virtualList; }
            set
            {
                _virtualList = value;
                RaisePropertyChangedEvent("VirtualList");
            }
        }
        /// <summary>
        /// zapytanie wyciągające pierwszą listę
        /// </summary>
        public string Query
        {
            get { return _currentQuery; }
            set
            {
                _currentQuery = value;
                RaisePropertyChangedEvent("Query");
            }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
        }
        /// <summary>
        /// ilość rekordów na ekranie
        /// </summary>
        public int RecordsOnScreen
        {
            get { return _recordsOnScreen; }
            set
            {
                _recordsOnScreen = value > Settings.Default.MaxRowAmount ? Settings.Default.MaxRowAmount : value;
                if (_recordsOnScreen == 0)
                    _recordsOnScreen = 1;
                RaisePropertyChangedEvent(new PropertyChangedEventArgs("RecordsOnScreen"));
            }
        }


        public bool IsExecuteButtonActive
        {
            get { return _isExecuteButtonActive; }
            set
            {
                _isExecuteButtonActive = value;

                RaisePropertyChangedEvent("IsExecuteButtonActive");
                RaisePropertyChangedEvent("IsCancelButtonActive");
            }
        }
        public bool IsNextButtonActive
        {
            get { return _isNextButtonActive; }
            set
            {
                _isNextButtonActive = value;

                RaisePropertyChangedEvent("IsNextButtonActive");
            }
        }
        public bool IsPreviousButtonActive
        {
            get { return _isPreviousButtonActive; }
            set
            {
                _isPreviousButtonActive = value;

                RaisePropertyChangedEvent("IsPreviousButtonActive");
            }
        }

        public string CurrentIndex
        {
            get { return _indexBar; }
            set
            {
                _indexBar = value;
                RaisePropertyChangedEvent("CurrentIndex");
            }
        }


        public ICommand ExecuteButtonClick
        {
            get { return new DelegateCommand(ExecuteButton_Click); }

        }

        public ICommand SearchButtonClick
        {
            get { return new DelegateCommand(SearchButton_Click); }

        }
        public ICommand CancelButtonClick
        {
            get { return new DelegateCommand(CancelReading); }

        }
        public ICommand NextButtonClick
        {
            get { return new DelegateCommand(NextButton_Click); }
        }
        public ICommand PreviousButtonClick
        {
            get { return new DelegateCommand(PreviousButton_Click); }
        }
        /// <summary>
        /// podnosi resize okna
        /// </summary>
        public ICommand SizeChanged
        {
            get { return new DelegateCommand(Resize); }
        }

        public bool IsCancelButtonActive
        {
            get { return !_isExecuteButtonActive; }
        }


        #endregion
        #region clicks
        int i = 0;
        public void ExecuteButton_Click()
        {

            _isVirtualListEmpty = true;
            DisableButtons();
            AmountOfRecordsInDatabase = _startFrom = 0;
            CurrentIndex = "Wczytywanie Danych";
            GetData();

        }
        public void SearchButton_Click()
        {
            _isVirtualListEmpty = true;
            DisableButtons();
            AmountOfRecordsInDatabase = _startFrom = 0;
            CurrentIndex = "Wczytywanie Danych";
            GetData(new SelectList(FromAge, ToAge, SName, SIllName, SSpecie));

        }
        public void NextButton_Click()
        {
            Resize();
            if (IsNextButtonActive)
            {
                DisableButtons();
                _startFrom += RecordsOnScreen;
                CurrentIndex = "Wczytywanie Danych";
                UpdateData();
            }
        }

        public void PreviousButton_Click()
        {
            Resize();
            if (IsPreviousButtonActive)
            {
                DisableButtons();
                _startFrom -= RecordsOnScreen;
                _startFrom = _startFrom < 0 ? 0 : _startFrom;
                CurrentIndex = "Wczytywanie Danych";
                UpdateData();
            }
        }

        /// <summary>
        /// ustala które buttony mogą być aktywne a które nie
        /// </summary>
        private void SetButtons()
        {
            IsNextButtonActive = (_startFrom + _recordsOnScreen < AmountOfRecordsInDatabase);
            IsPreviousButtonActive = (_startFrom > 0);
            SetIndex();
            _oldRecordsOnScreen = RecordsOnScreen;
            IsExecuteButtonActive = _backgroundDataReader == null || !_backgroundDataReader.IsBusy;
        }

        /// <summary>
        /// wyświetla index listy
        /// </summary>
        private void SetIndex()
        {
            if (_virtualList != null)
                CurrentIndex = (_startFrom) + " - " +
                               Math.Min(AmountOfRecordsInDatabase, _startFrom + (RecordsOnScreen > _virtualList.Count
                                   ? _virtualList.Count
                                   : RecordsOnScreen));
        }

        /// <summary>
        /// wyłącza przyciski
        /// </summary>
        private void DisableButtons()
        {
            IsNextButtonActive = false;
            IsPreviousButtonActive = false;
            IsExecuteButtonActive = false;
        }

        /// <summary>
        /// odświeża listę widocznych elementów listy
        /// </summary>
        public void Resize()
        {
            if (_oldRecordsOnScreen != RecordsOnScreen && VirtualList != null)
            {
                DisableButtons();
                _oldRecordsOnScreen = RecordsOnScreen;
                CurrentIndex = "Wczytywanie Danych";
                UpdateData();
            }
        }
        #endregion

        /// <summary>
        /// próbuje pobrać wszystkie obiekty z tablicy
        /// </summary>
        private void GetData()
        {
            try
            {
                if (BoolCancelReading())
                    return;
                DataReader = new DataProvider(Query,
                    new DbConnectionFactory().CreateDbConnection(_currentDriver, ConnectionString),
                    BackupData, new DataFromDatabaseReader(), _maxChunkSize, _recordsOnScreen);
                RunBackgroundReader();

            }
            catch (Exception)
            {
                CurrentIndex = "wystąpił błąd.";
                IsExecuteButtonActive = true;
            }
        }
        /// <summary>
        /// pobiera listę z określonymi kryteriami wyszukiwania
        /// </summary>
        /// <param name="list"></param>
        private void GetData(SelectList list)
        {
            try
            {
                if (BoolCancelReading())
                    return;
                DataReader = new DataProvider(list,
                    new DbConnectionFactory().CreateDbConnection(_currentDriver, ConnectionString),
                    BackupData, new DataFromDatabaseReader(), _maxChunkSize, _recordsOnScreen, new Sqlparser());
                RunBackgroundReader();

            }
            catch (Exception)
            {
                CurrentIndex = "wystąpił błąd.";
                IsExecuteButtonActive = true;
            }
        }
        
        protected void UpdateData()
        {
            try
            {
                StartVirtualList();
            }
            catch (Exception)
            {
                CurrentIndex = "wystąpił błąd.";
                IsExecuteButtonActive = true;
                throw;
            }
        }
        /// <summary>
        /// resetuje datagrid
        /// </summary>
        private void StartVirtualList()
        {
            int recordsToShow;
            if (_backgroundDataReader != null && _backgroundDataReader.IsBusy)
                recordsToShow = RecordsOnScreen;
            else
                recordsToShow = RecordsOnScreen > RecordsCached - _startFrom ? RecordsCached - _startFrom : RecordsOnScreen;
            VirtualList = new VirtualizedPageGrid(_startFrom, DataReader, recordsToShow, BackupData);
            _isVirtualListEmpty = false;
            SetButtons();
            Columns = _virtualList.GetColumns();
        }

        #region backgrounreader
        /// <summary>
        /// przerywa backgroundreadera
        /// </summary>
        private void CancelReading()
        {
            BoolCancelReading();
        }
        /// <summary>
        /// przerywa backgroundreadera
        /// </summary>
        private bool BoolCancelReading()
        {
            if (_backgroundDataReader != null && _backgroundDataReader.IsBusy)
            {
                SetButtons();
                _backgroundDataReader.CancelAsync();
                return true;
            }
            return false;
        }
        /// <summary>
        /// uruchamia backgroundreadera do pobierania danych
        /// </summary>
        private void RunBackgroundReader()
        {
            _backgroundDataReader = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _backgroundDataReader.DoWork += DataReader.InitializeReading;
            _backgroundDataReader.ProgressChanged += BackgroundWorkerProgress;
            _backgroundDataReader.RunWorkerCompleted += BackgroundWorkerCompleated;
            _backgroundDataReader.RunWorkerAsync();
        }
        /// <summary>
        /// informuje o postępie backgroundreadera
        /// </summary>
        private void BackgroundWorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            if (_isVirtualListEmpty && AmountOfRecordsInDatabase > 0)
                StartVirtualList();
            RecordsCached = e.ProgressPercentage;
        }
        /// <summary>
        /// zamyka backgroundreadera
        /// </summary>
        private void BackgroundWorkerCompleated(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isVirtualListEmpty && AmountOfRecordsInDatabase > 0)
                StartVirtualList();
            if (RecordsCached < _startFrom + RecordsOnScreen)
                VirtualList.TrimSize(RecordsCached - _startFrom);
            SetButtons();
        }
        #endregion
    }
}
