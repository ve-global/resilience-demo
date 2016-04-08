using GalaSoft.MvvmLight.CommandWpf;
using ResilienceDemo.Client.Components;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ResilienceDemo.Services;
using System.Collections.Generic;

namespace ResilienceDemo.Client.ViewModel
{
    public class SpammerViewModel : INotifyPropertyChanged
    {
        // Subjects
        private Subject<ClientEvent> _clientSubject;
        private Subject<ServerEvent> _serverSubject;

        private SynchronisedEventFeed _eventFeed;

        // Services
        private ITestService _rawService;
        private ITestService _retryService;
        private ITestService _retryCbService;

        // Settings
        private ServiceOption _selectedOption;
        private int _callsPerSecond;
        private int _secondsDuration;
        private bool _run = false;

        // Client Value Holders
        private int _clientRequestsMade = 0;
        private int _clientRequestsCompleted = 0;
        private int _clientRequestsFailed = 0;

        // Server Value Holders
        private int _serverRequestsMade = 0;
        private int _serverRequestsCompleted = 0;
        private int _serverRequestsFailed = 0;
        private int _serverRequestsRetried = 0;
        private Spammer _spammer;

        public SpammerViewModel()
        {
            _callsPerSecond = 50;
            _secondsDuration = 30;

            ConfigureServerSubject();
            ConfigureClientSubject();
            BootstrapServices();

            ServiceOptions = new List<ServiceOption>()
            {
                new ServiceOption("Raw", _rawService),
                new ServiceOption("Retry", _retryService),
                new ServiceOption("Retry With Circuit Breaker", _retryCbService),
            };
            SelectedServiceOption = ServiceOptions.First();

            SpamCommand = new RelayCommand(ExecuteSpam, CanExecuteSpam);
        }

        private void ConfigureClientSubject()
        {
            _clientSubject = new Subject<ClientEvent>();
            _clientSubject
                .Where(e => e.Status == ClientEventStatus.Started)
                //.Buffer(TimeSpan.FromSeconds(0.5))
                //.ObserveOnDispatcher()
                //.Subscribe(e => ClientRequestsMade += e.Count);
                .Subscribe(e => ClientRequestsMade += 1);

            _clientSubject
                .Where(e => e.Status == ClientEventStatus.Completed)
                //.Buffer(TimeSpan.FromSeconds(0.5))
                //.ObserveOnDispatcher()
                //.Subscribe(e => ClientRequestsCompleted += e.Count);
                .Subscribe(e => ClientRequestsCompleted += 1);

            _clientSubject
                .Where(e => e.Status == ClientEventStatus.Failed)
                //.Buffer(TimeSpan.FromSeconds(0.5))
                //.ObserveOnDispatcher()
                //.Subscribe(e => ClientRequestsFailed += e.Count);
                .Subscribe(e => ClientRequestsFailed += 1);
        }

        private void ConfigureServerSubject()
        {
            _serverSubject = new Subject<ServerEvent>();
            _serverSubject
                .Where(e => e.Status == ServerEventStatus.Started)
                .Subscribe(e => ServerRequestsMade += 1);

            _serverSubject
                .Where(e => e.Status == ServerEventStatus.Completed)
                .Subscribe(e => ServerRequestsCompleted += 1);

            _serverSubject
                .Where(e => e.Status == ServerEventStatus.Failed)
                .Subscribe(e => ServerRequestsFailed += 1);

            _serverSubject
                .Where(e => e.Status == ServerEventStatus.Retried)
                .Subscribe(e => ServerRequestsRetried += 1);
        }

        private void BootstrapServices()
        {
            _eventFeed = new SynchronisedEventFeed(_clientSubject, _serverSubject);
            _spammer = new Spammer(_eventFeed);

            _rawService = new IntermittentService(_eventFeed);

            _retryService = new RetryService(
                new IntermittentService(_eventFeed), _eventFeed);

            _retryCbService = new RetryService(
                new CircuitBreakerService(
                    new IntermittentService(_eventFeed)),
                    _eventFeed);
        }

        public ICommand SpamCommand { get; private set; }


        public IEnumerable<ServiceOption> ServiceOptions { get; private set; }

        public ServiceOption SelectedServiceOption
        {
            get { return _selectedOption; }

            set
            {
                if (_selectedOption == value)
                    return;

                _selectedOption = value;
                OnPropertyChanged();
            }
        }

        public int CallsPerSecond
        {
            get { return _callsPerSecond; }
            set
            {
                _callsPerSecond = value;
                OnPropertyChanged();
            }
        }

        public int SecondsDuration
        {
            get { return _secondsDuration; }
            set
            {
                _secondsDuration = value;
                OnPropertyChanged();
            }
        }


        public int ClientRequestsMade
        {
            get
            {
                return _clientRequestsMade;
            }
            set
            {
                if (_clientRequestsMade == value)
                    return;

                _clientRequestsMade = value;
                OnPropertyChanged();
            }
        }

        public int ClientRequestsCompleted
        {
            get
            {
                return _clientRequestsCompleted;
            }
            set
            {
                if (_clientRequestsCompleted == value)
                    return;

                _clientRequestsCompleted = value;
                OnPropertyChanged();
            }
        }

        public int ClientRequestsFailed
        {
            get
            {
                return _clientRequestsFailed;
            }
            set
            {
                if (_clientRequestsFailed == value)
                    return;

                _clientRequestsFailed = value;
                OnPropertyChanged();
            }
        }


        public int ServerRequestsMade
        {
            get { return _serverRequestsMade; }
            set
            {
                if (_serverRequestsMade == value)
                    return;

                _serverRequestsMade = value;
                OnPropertyChanged();
            }
        }

        public int ServerRequestsCompleted
        {
            get { return _serverRequestsCompleted; }
            set
            {
                if (_serverRequestsCompleted == value)
                    return;

                _serverRequestsCompleted = value;
                OnPropertyChanged();
            }
        }

        public int ServerRequestsFailed
        {
            get { return _serverRequestsFailed; }
            set
            {
                if (_serverRequestsFailed == value)
                    return;

                _serverRequestsFailed = value;
                OnPropertyChanged();
            }
        }

        public int ServerRequestsRetried
        {
            get { return _serverRequestsRetried; }
            set
            {
                if (_serverRequestsRetried == value)
                    return;

                _serverRequestsRetried = value;
                OnPropertyChanged();
            }
        }


        private bool CanExecuteSpam()
        {
            return !_run ||
                (ClientRequestsCompleted + ClientRequestsFailed) ==
                    (CallsPerSecond * SecondsDuration);
        }

        private void ExecuteSpam()
        {
            ClientRequestsMade = 0;
            ClientRequestsCompleted = 0;
            ClientRequestsFailed = 0;

            ServerRequestsMade = 0;
            ServerRequestsCompleted = 0;
            ServerRequestsFailed = 0;
            ServerRequestsRetried = 0;

            _run = true;

            _spammer.ExecuteApi(SelectedServiceOption.Service.Execute, CallsPerSecond, SecondsDuration);
        }


        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}