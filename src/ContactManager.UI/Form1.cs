using ContactManager.App.Models;
using ContactManager.App.Persistence;

namespace ContactManager.UI;

public partial class Form1 : Form
{
    private readonly IContactRepository _repository = new JsonContactRepository("contacts.json");

    private ListBox lstContacts;
    private TextBox txtName, txtCompany, txtPhone, txtEmail, txtAddress;
    private Button btnAdd, btnUpdate, btnDelete;

    public Form1()
    {
        InitializeComponent();
        BuildLayout();
        LoadContacts();
    }

    private void BuildLayout()
    {
        // --- Panels ---
        var leftPanel = new Panel()
        {
            Dock = DockStyle.Left,
            Width = 320, // <<< breiter gemacht
            Padding = new Padding(10)
        };

        var rightPanel = new Panel()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };

        Controls.Add(rightPanel);
        Controls.Add(leftPanel);

        // --- Listbox ---
        lstContacts = new ListBox()
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11) // größere Schrift für bessere Lesbarkeit
        };
        lstContacts.SelectedIndexChanged += lstContacts_SelectedIndexChanged;

        leftPanel.Controls.Add(lstContacts);

        // --- Search Panel ---
        var searchPanel = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.LeftToRight,
            Dock = DockStyle.Top,
            AutoSize = true
        };

        var txtSearch = new TextBox()
        {
            Width = 200,
            Font = new Font("Segoe UI", 10)
        };

        var btnSearch = new Button()
        {
            Text = "Search",
            Width = 140,
            Height = 40,
            Font = new Font("Segoe UI", 10),
            TextAlign = ContentAlignment.MiddleCenter
        };

        btnSearch.Click += (s, e) =>
        {
            var searchValue = txtSearch.Text.Trim().ToLower();

            lstContacts.Items.Clear();

            foreach (var c in _repository.GetAll()
                .Where(c => 
                    c.Name.ToLower().Contains(searchValue) ||
                    c.Company.ToLower().Contains(searchValue)))
            {
                lstContacts.Items.Add(c);
            }

            lstContacts.DisplayMember = "Name";
        };

        rightPanel.Controls.Add(searchPanel);
        searchPanel.Controls.Add(txtSearch);
        searchPanel.Controls.Add(btnSearch);

        // --- Table Layout for fields ---
        var layout = new TableLayoutPanel()
        {
            RowCount = 7,
            ColumnCount = 2,
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(0, 10, 0, 10)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // <<< breitere Label-Spalte
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300)); // <<< schöne Textfeld-Breite

        rightPanel.Controls.Add(layout);

        // Helper to add row
        void AddRow(string labelText, out TextBox textBox)
        {
            var label = new Label()
            {
                Text = labelText,
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            textBox = new TextBox()
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Left
            };

            layout.Controls.Add(label);
            layout.Controls.Add(textBox);
        }

        AddRow("Name:", out txtName);
        AddRow("Company:", out txtCompany);
        AddRow("Phone:", out txtPhone);
        AddRow("Email:", out txtEmail);
        AddRow("Address:", out txtAddress);

        // --- Buttons ---
        var buttonPanel = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.LeftToRight,
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(0, 25, 0, 0)
        };

        rightPanel.Controls.Add(buttonPanel);

        btnAdd = new Button() 
        { 
            Text = "Add", 
            Width = 140, 
            Height = 40 
        };

        btnUpdate = new Button() 
        { 
            Text = "Update", 
            Width = 140, 
            Height = 40 
        };

        btnDelete = new Button() 
        { 
            Text = "Delete", 
            Width = 140, 
            Height = 40 
        };

        // Textgröße bleibt bewusst klein
        var buttonFont = new Font("Segoe UI", 10, FontStyle.Regular);
        btnAdd.Font = buttonFont;
        btnUpdate.Font = buttonFont;
        btnDelete.Font = buttonFont;

        btnAdd.Click += btnAdd_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;

        buttonPanel.Controls.Add(btnAdd);
        buttonPanel.Controls.Add(btnUpdate);
        buttonPanel.Controls.Add(btnDelete);
    }


    private void LoadContacts()
    {
        lstContacts.Items.Clear();
        foreach (var c in _repository.GetAll())
            lstContacts.Items.Add(c);

        lstContacts.DisplayMember = "Name";
    }

    private void lstContacts_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstContacts.SelectedItem is not Contact c) return;

        txtName.Text = c.Name;
        txtCompany.Text = c.Company;
        txtPhone.Text = c.Phone;
        txtEmail.Text = c.Email;
        txtAddress.Text = c.Address;
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        var error = ValidateContactInput();
        if (error != "")
        {
            MessageBox.Show(error, "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var c = new Contact
        {
            Name = txtName.Text,
            Company = txtCompany.Text,
            Phone = txtPhone.Text,
            Email = txtEmail.Text,
            Address = txtAddress.Text
        };

        _repository.Add(c);
        LoadContacts();
        MessageBox.Show("Kontakt erfolgreich hinzugefügt.", "Erfolg");
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (lstContacts.SelectedItem is not Contact c)
        {
            MessageBox.Show("Bitte zuerst einen Kontakt auswählen.", "Hinweis");
            return;
        }

        var error = ValidateContactInput();
        if (error != "")
        {
            MessageBox.Show(error, "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        c.Name = txtName.Text;
        c.Company = txtCompany.Text;
        c.Phone = txtPhone.Text;
        c.Email = txtEmail.Text;
        c.Address = txtAddress.Text;

        _repository.Update(c);
        LoadContacts();
        MessageBox.Show("Kontakt erfolgreich aktualisiert.", "Erfolg");
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (lstContacts.SelectedItem is not Contact c)
        {
            MessageBox.Show("Bitte zuerst einen Kontakt auswählen.", "Hinweis");
            return;
        }

        var confirmed = MessageBox.Show(
            "Wirklich löschen?", 
            "Bestätigung",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmed == DialogResult.Yes)
        {
            _repository.Delete(c.Id);
            LoadContacts();
        }
    }


    private string ValidateContactInput()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
            return "Name darf nicht leer sein.";

        if (string.IsNullOrWhiteSpace(txtPhone.Text))
            return "Phone darf nicht leer sein.";

        // Simple email validation
        if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
            !txtEmail.Text.Contains("@") ||
            !txtEmail.Text.Contains(".") ||
            txtEmail.Text.Contains(" "))
            return "Bitte gültige Email eingeben.";

        return ""; // OK
    }

}
