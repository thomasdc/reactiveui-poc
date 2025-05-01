using ConsoleApp;
using ConsoleApp.Bla;
using ReactiveUI;

// https://www.youtube.com/watch?v=IH2yx7b9DNY
var viewModel = new ConsoleViewModel();
viewModel.WhenAnyValue(_ => _.TimeAsString)
    .Subscribe(Console.WriteLine);

Console.ReadLine();