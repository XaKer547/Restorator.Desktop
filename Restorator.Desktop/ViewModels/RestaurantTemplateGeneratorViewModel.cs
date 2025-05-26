using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Extensions;
using Restorator.Desktop.Models;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Templates;
using Restorator.Domain.Services;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantTemplateGeneratorViewModel : ViewModelBase
    {
        private readonly ITemplateService _templateService;
        public RestaurantTemplateGeneratorViewModel(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        private const string SchemeLocation =
            "C:\\Users\\user\\source\\repos\\XaKer547\\Restorator\\Restorator.Seeder\\Resources\\RestaurantsPlan";

        //тут свой путь к сидеру пиши

        [ObservableProperty]
        private ObservableCollection<TableModel> tables = [];

        [ObservableProperty]
        private ObservableCollection<RestaurantTemplatePreview> templates = [];

        [ObservableProperty]
        private ObservableCollection<TableModel> tableTemplates = [];

        [ObservableProperty]
        private TableModel selectedTableTempate;

        [ObservableProperty]
        private TableModel? selectedTable;

        [ObservableProperty]
        private TemplateModel selectedTemplate;

        [ObservableProperty]
        private string seederScript;

        [ObservableProperty]
        private bool canChangeTable = false;

        [RelayCommand]
        public async Task Initialize()
        {
            var teplates = await _templateService.GetTableTemplates();

            foreach (var tableTemplate in teplates)
                TableTemplates.Add(tableTemplate.ToModel());

            SelectedTableTempate = TableTemplates[0];

            var schemes = await _templateService.GetRestaurantsTemplatePreview();

            foreach (var scheme in schemes)
            {
                Templates.Add(scheme);
            }

            AddNewTable();
        }

        [RelayCommand]
        public async Task<IReadOnlyCollection<TableModel>> LoadTableTemplates()
        {
            await Task.Delay(1); //IDK :)

            return
            [
                new TableModel()
                {
                    State = Domain.Models.Enums.TableStates.OccupiedByUser,
                    Height = 100,
                    Width = 108,
                    Rotation = 0,
                    TemplateId = 1,
                },
                new TableModel()
                {
                    State = Domain.Models.Enums.TableStates.OccupiedByUser,
                    Height = 183,
                    Width = 183,
                    Rotation = 0,
                    TemplateId = 1,
                },
                new TableModel()
                {
                    State = Domain.Models.Enums.TableStates.OccupiedByUser,
                    Height = 183,
                    Width = 110,
                    Rotation = 0,
                    TemplateId = 1,
                },
            ];
        }

        [RelayCommand]
        public void ChangeSelectedTable(TableModel table)
        {
            SelectedTable = table;

            CanChangeTable = true;
        }

        [RelayCommand]
        public void AddNewTable()
        {
            var table = SelectedTableTempate.Clone();

            SelectedTable = table;

            CanChangeTable = true;

            Tables.Add(table);
        }

        [RelayCommand]
        public void ClearScheme() => Tables.Clear();

        [RelayCommand]
        public void ChangeSelectedTableTemplate(TableModel template)
        {
            SelectedTableTempate = template;

            if (SelectedTable is null)
                return;

            SelectedTable.Height = SelectedTableTempate.Height;
            SelectedTable.Width = SelectedTableTempate.Width;
        }

        [RelayCommand]
        public void RemoveTable()
        {
            Tables.Remove(SelectedTable!);

            SelectedTable = null;

            CanChangeTable = false;
        }

        /*
        private TemplateModel GetTemplateFromPath(string imagePath)
        {
            return new TemplateModel()
            {
                Filename = Path.GetFileName(imagePath),
                Content = File.ReadAllBytes(imagePath),
            };
        }

        [RelayCommand]
        public void CopyScriptToBuffer() => Clipboard.SetText(SeederScript);

        [RelayCommand]
        public void GenerateSeederScript()
        {
            var script = new StringBuilder("new RestaurantTemplate\n{\n");

            var fileNameWithoutExtension = SelectedTemplate.Filename.Split('.')[0];

            script.AppendLine(
                $"\tImage = EmbeddedResourceHelper.GetRestaurantReservationPlan(\"{fileNameWithoutExtension}\"),"
            );

            script.AppendLine("\tTables = new List<Table>()\n\t\t{");

            foreach (var table in Tables)
            {
                script.AppendLine("\t\t\tnew Table()\n\t\t\t{");


                script.AppendLine($"\t\t\t\tTableTemplateId = {table.TemplateId},");

                //у меня margin стоит для того чтобы FlipView отработал
                script.AppendLine(
                    CultureInfo.InvariantCulture,
                    $"\t\t\t\tX = {Math.Round(table.X, 2) + 10}F,"
                );

                script.AppendLine(
                    CultureInfo.InvariantCulture,
                    $"\t\t\t\tY = {Math.Round(table.Y, 2)}F,"
                );

                script.AppendLine("\t\t\t},");
            }

            script.AppendLine("\t\t}");
            script.AppendLine("},");

            SeederScript = script.ToString();
        }
        */
    }
}