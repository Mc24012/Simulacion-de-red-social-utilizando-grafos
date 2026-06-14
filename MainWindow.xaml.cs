using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Simulacion_de_red_social_utilizando_grafos.BaseDatos;
using Simulacion_de_red_social_utilizando_grafos.Clases;
using Simulacion_de_red_social_utilizando_grafos.DAOs;

namespace Simulacion_de_red_social_utilizando_grafos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grafo _grafo = new Grafo();
        private readonly List<Usuario> _usuarios = [];
        private readonly HashSet<(int Seguidor, int Seguido)> _relaciones = [];
        private readonly Dictionary<int, Point> _posicionesGrafo = [];
        private readonly ObservableCollection<CambioHistorial> _historialCambios = [];
        private readonly UsuarioDAO _usuarioDAO = new(Conexion.CrearConexion);
        private readonly RelacionDAO _relacionDAO = new(Conexion.CrearConexion);
        private readonly InteresDAO _interesDAO = new(Conexion.CrearConexion);
        private Dictionary<int, List<string>> _interesesUsuarios = [];
        private List<(int Id, string Nombre)> _catalogoIntereses = [];
        private HashSet<(int Origen, int Destino, string Interes)> _posiblesRelaciones = [];
        private Border? _tarjetaArrastrando;
        private Point _offsetArrastre;
        private bool _redibujandoPorTamano;
        private bool _baseDatosConectada;

        public MainWindow()
        {
            InitializeComponent();
            dgHistorial.ItemsSource = _historialCambios;
            CargarDatosDesdeBaseDatos(mostrarMensaje: false);
        }

        private void btnCargarBD_Click(object sender, RoutedEventArgs e)
        {
            CargarDatosDesdeBaseDatos(mostrarMensaje: true);
        }

        private void btnVisualizarGrafo_Click(object sender, RoutedEventArgs e)
        {
            var ventanaGrafo = new VentanaGrafo(_usuarios, _relaciones, _posiblesRelaciones, _interesesUsuarios)
            {
                Owner = this
            };

            ventanaGrafo.Show();
        }

        private void btnSeccionUsuarios_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionUsuarios, panelNuevoUsuario);
            ActivarBotonVista(btnVistaNuevoUsuario);
            txtNombre.Focus();
        }

        private void btnSeccionRelaciones_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionRelaciones, panelRelaciones);
            ActivarBotonVista(btnVistaRelaciones);
            cmbSeguidorRelacion.Focus();
        }

        private void btnSeccionGrafo_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionGrafo, panelVisualizacionGrafo);
            RestablecerBotonesVista();
            DibujarGrafo();
        }

        private void btnSeccionSugerencias_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionSugerencias, panelSugerencias);
            ActivarBotonVista(btnVistaSugerencias);

            if (ObtenerUsuarioSugerenciasSeleccionado() is Usuario usuario)
            {
                ActualizarSugerencias(usuario);
            }

            cmbUsuarioSugerencias.Focus();
        }

        private void btnSeccionHistorial_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionHistorial, panelHistorial);
            ActivarBotonVista(btnVistaHistorial);
            dgHistorial.Focus();
        }

        private void btnVistaNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionUsuarios, panelNuevoUsuario);
            ActivarBotonVista(btnVistaNuevoUsuario);
            txtNombre.Focus();
        }

        private void btnVistaUsuariosActivos_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionUsuarios, panelUsuariosActivos);
            ActivarBotonVista(btnVistaUsuariosActivos);
            dgUsuarios.Focus();
        }

        private void btnVistaSugerencias_Click(object sender, RoutedEventArgs e)
        {
            btnSeccionSugerencias_Click(sender, e);
        }

        private void btnVistaRelaciones_Click(object sender, RoutedEventArgs e)
        {
            btnSeccionRelaciones_Click(sender, e);
        }

        private void btnVistaGustos_Click(object sender, RoutedEventArgs e)
        {
            ActivarSeccion(btnSeccionUsuarios, panelIntereses);
            ActivarBotonVista(btnVistaGustos);

            if (ObtenerUsuarioInteresesSeleccionado() is Usuario usuario)
            {
                CargarInteresesUsuarioSeleccionado(usuario);
            }

            cmbUsuarioIntereses.Focus();
        }

        private void btnVistaHistorial_Click(object sender, RoutedEventArgs e)
        {
            btnSeccionHistorial_Click(sender, e);
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var nombre = txtNombre.Text.Trim();
            var email = txtEmail.Text.Trim();
            var genero = ObtenerGeneroSeleccionado();

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Ingrese nombre y email del usuario.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!NombreEsValido(nombre))
            {
                MessageBox.Show("El nombre solo puede contener letras y espacios. No se permiten numeros, simbolos ni emojis.", "Nombre invalido", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombre.Focus();
                return;
            }

            if (!EmailEsValido(email))
            {
                MessageBox.Show("Ingrese un email valido. Ejemplo: usuario@dominio.com", "Email invalido", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            if (!_baseDatosConectada)
            {
                MessageBox.Show("No hay conexion con la base de datos. El usuario no se agrego. Encienda MySQL/MariaDB y presione Cargar datos.", "Base de datos desconectada", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _usuarioDAO.Insertar(new Modelos.Usuario(0, nombre, genero, email));
                RegistrarCambio("Usuario agregado", $"Se agrego el usuario {nombre} ({genero}) con email {email}.");
                CargarDatosDesdeBaseDatos(mostrarMensaje: false);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo insertar el usuario en la base de datos. No se agrego a la interfaz.\n\n{ex.Message}", "Base de datos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardarIntereses_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarConexionBaseDatos())
            {
                return;
            }

            if (ObtenerUsuarioInteresesSeleccionado() is not Usuario usuario)
            {
                MessageBox.Show("Seleccione un usuario para guardar sus gustos.", "Gustos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var seleccionados = lstIntereses.Items
                .OfType<InteresSeleccionable>()
                .Where(interes => interes.Seleccionado)
                .Select(interes => interes.Id)
                .ToList();

            if (seleccionados.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un gusto para el usuario.", "Gustos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var interesesAntes = _interesesUsuarios.TryGetValue(usuario.Id_Usuario, out var actuales)
                    ? actuales.ToHashSet(StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var interesesDespues = _catalogoIntereses
                    .Where(interes => seleccionados.Contains(interes.Id))
                    .Select(interes => interes.Nombre)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                var agregados = interesesDespues.Except(interesesAntes, StringComparer.OrdinalIgnoreCase).OrderBy(nombreInteres => nombreInteres).ToList();
                var quitados = interesesAntes.Except(interesesDespues, StringComparer.OrdinalIgnoreCase).OrderBy(nombreInteres => nombreInteres).ToList();

                _interesDAO.GuardarInteresesUsuario(usuario.Id_Usuario, seleccionados);
                RegistrarCambioGustos(usuario, agregados, quitados);
                CargarDatosDesdeBaseDatos(mostrarMensaje: false);
                SeleccionarUsuarioEnCombos(usuario.Id_Usuario);
                MessageBox.Show("Gustos guardados correctamente.", "Gustos", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron guardar los gustos.\n\n{ex.Message}", "Gustos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAgregarSugerencia_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarConexionBaseDatos())
            {
                return;
            }

            if (ObtenerUsuarioSugerenciasSeleccionado() is not Usuario seguidor)
            {
                MessageBox.Show("Seleccione un usuario para agregar la relacion.", "Sugerencias", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (lstSugerencias.SelectedItem is not SugerenciaUsuario sugerencia)
            {
                MessageBox.Show("Seleccione una sugerencia de la lista.", "Sugerencias", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AgregarRelacion(seguidor, sugerencia.Usuario, "Sugerencia agregada"))
            {
                MessageBox.Show("Relacion agregada correctamente.", "Sugerencias", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnBFS_Click(object sender, RoutedEventArgs e)
        {
            if (ObtenerUsuarioSeleccionado() is not Usuario usuario)
            {
                MessageBox.Show("Seleccione un usuario para ejecutar BFS.", "BFS", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var recorrido = _grafo.BFS(usuario);
            MostrarRecorrido("BFS", recorrido);
        }

        private void btnDFS_Click(object sender, RoutedEventArgs e)
        {
            if (ObtenerUsuarioSeleccionado() is not Usuario usuario)
            {
                MessageBox.Show("Seleccione un usuario para ejecutar DFS.", "DFS", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var recorrido = _grafo.DFS(usuario);
            MostrarRecorrido("DFS", recorrido);
        }

        private void btnSeguir_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarConexionBaseDatos())
            {
                return;
            }

            if (cmbSeguidorRelacion.SelectedItem is not Usuario seguidor ||
                cmbSeguidoRelacion.SelectedItem is not Usuario seguido)
            {
                MessageBox.Show("Seleccione seguidor y seguido.", "Relacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (seguidor == seguido)
            {
                MessageBox.Show("Un usuario no puede seguirse a si mismo.", "Relacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AgregarRelacion(seguidor, seguido, "Relacion agregada");
        }

        private bool AgregarRelacion(Usuario seguidor, Usuario seguido, string tipoHistorial)
        {
            if (seguidor == seguido)
            {
                MessageBox.Show("Un usuario no puede seguirse a si mismo.", "Relacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_relaciones.Add((seguidor.Id_Usuario, seguido.Id_Usuario)))
            {
                MessageBox.Show("La relacion ya existe.", "Relacion", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (_baseDatosConectada)
            {
                try
                {
                    _relacionDAO.Insertar(new Modelos.Relacion(
                        ConvertirUsuarioModelo(seguidor),
                        ConvertirUsuarioModelo(seguido),
                        DateTime.Now));
                    RegistrarCambio(tipoHistorial, $"{seguidor.Nombre_Usuario} ahora sigue a {seguido.Nombre_Usuario}.");
                    CargarDatosDesdeBaseDatos(mostrarMensaje: false);
                    SeleccionarUsuarioEnCombos(seguidor.Id_Usuario);
                    return true;
                }
                catch (Exception ex)
                {
                    _relaciones.Remove((seguidor.Id_Usuario, seguido.Id_Usuario));
                    MessageBox.Show($"No se pudo guardar la relacion en la base de datos.\n\n{ex.Message}", "Base de datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            _grafo.Seguir(seguidor, seguido);
            RegistrarCambio(tipoHistorial, $"{seguidor.Nombre_Usuario} ahora sigue a {seguido.Nombre_Usuario}.");
            ActualizarSugerencias(seguidor);
            DibujarGrafo();
            return true;
        }

        private void btnDejar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarConexionBaseDatos())
            {
                return;
            }

            if (cmbSeguidorRelacion.SelectedItem is not Usuario seguidor ||
                cmbSeguidoRelacion.SelectedItem is not Usuario seguido)
            {
                MessageBox.Show("Seleccione seguidor y seguido.", "Relacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_relaciones.Remove((seguidor.Id_Usuario, seguido.Id_Usuario)))
            {
                if (_baseDatosConectada)
                {
                    try
                    {
                        _relacionDAO.Eliminar(seguidor.Id_Usuario, seguido.Id_Usuario);
                        RegistrarCambio("Relacion eliminada", $"{seguidor.Nombre_Usuario} dejo de seguir a {seguido.Nombre_Usuario}.");
                        CargarDatosDesdeBaseDatos(mostrarMensaje: false);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _relaciones.Add((seguidor.Id_Usuario, seguido.Id_Usuario));
                        MessageBox.Show($"No se pudo eliminar la relacion en la base de datos.\n\n{ex.Message}", "Base de datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                _grafo.DejarSeguir(seguidor, seguido);
                RegistrarCambio("Relacion eliminada", $"{seguidor.Nombre_Usuario} dejo de seguir a {seguido.Nombre_Usuario}.");
                ActualizarSugerencias(seguidor);
                DibujarGrafo();
            }
        }

        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsuarios.SelectedItem is Usuario usuario)
            {
                cmbSeguidorRelacion.SelectedItem = usuario;
                cmbUsuarioSugerencias.SelectedItem = usuario;
                cmbUsuarioIntereses.SelectedItem = usuario;
                ActualizarSugerencias(usuario);
                CargarInteresesUsuarioSeleccionado(usuario);
            }
        }

        private void cmbUsuarioSugerencias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUsuarioSugerencias.SelectedItem is Usuario usuario)
            {
                ActualizarSugerencias(usuario);
            }
        }

        private void cmbUsuarioIntereses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUsuarioIntereses.SelectedItem is Usuario usuario)
            {
                CargarInteresesUsuarioSeleccionado(usuario);
            }
        }

        private void CargarDatosDesdeBaseDatos(bool mostrarMensaje)
        {
            try
            {
                ReiniciarDatos();
                _interesDAO.AsegurarTablasYDatos();

                var usuariosBD = _usuarioDAO.ObtenerTodos();

                foreach (var usuario in usuariosBD)
                {
                    _usuarios.Add(usuario);
                    _grafo.AgregarUsuario(usuario);
                }

                var usuariosPorId = _usuarios.ToDictionary(usuario => usuario.Id_Usuario);

                foreach (var relacion in _relacionDAO.ObtenerTodas())
                {
                    if (!usuariosPorId.TryGetValue(relacion.Id_Seguidor, out var seguidor) ||
                        !usuariosPorId.TryGetValue(relacion.Id_Seguido, out var seguido))
                    {
                        continue;
                    }

                    if (_relaciones.Add((seguidor.Id_Usuario, seguido.Id_Usuario)))
                    {
                        _grafo.Seguir(seguidor, seguido);
                    }
                }

                _interesesUsuarios = _interesDAO.ObtenerInteresesPorUsuario();
                _catalogoIntereses = _interesDAO.ObtenerTodos();
                _posiblesRelaciones = _interesDAO.ObtenerPosiblesRelacionesPorIntereses(_relaciones);
                _interesDAO.GuardarPosiblesRelaciones(_posiblesRelaciones);
                _baseDatosConectada = true;
                ActualizarEstadoControles();
                ActualizarInterfaz();
                DibujarGrafo();

                if (mostrarMensaje)
                {
                    MessageBox.Show("Datos cargados desde red_db correctamente.", "Base de datos", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _baseDatosConectada = false;
                ActualizarEstadoControles();
                ReiniciarDatos();
                ActualizarInterfaz();
                DibujarGrafo();

                if (mostrarMensaje)
                {
                    MessageBox.Show($"No se pudo conectar con red_db. No se cargaron datos locales porque la aplicacion ahora trabaja conectada a la base de datos.\n\n{ex.Message}", "Base de datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ReiniciarDatos()
        {
            _usuarios.Clear();
            _relaciones.Clear();
            _posicionesGrafo.Clear();
            _interesesUsuarios = [];
            _catalogoIntereses = [];
            _posiblesRelaciones = [];
            _grafo = new Grafo();
        }

        private void ActualizarInterfaz()
        {
            dgUsuarios.ItemsSource = null;
            dgUsuarios.ItemsSource = _usuarios;

            cmbSeguidorRelacion.ItemsSource = null;
            cmbSeguidoRelacion.ItemsSource = null;
            cmbUsuarioSugerencias.ItemsSource = null;
            cmbUsuarioIntereses.ItemsSource = null;
            cmbSeguidorRelacion.ItemsSource = _usuarios;
            cmbSeguidoRelacion.ItemsSource = _usuarios;
            cmbUsuarioSugerencias.ItemsSource = _usuarios;
            cmbUsuarioIntereses.ItemsSource = _usuarios;

            if (_usuarios.Count > 0)
            {
                dgUsuarios.SelectedIndex = 0;
                cmbSeguidorRelacion.SelectedIndex = 0;
                cmbSeguidoRelacion.SelectedIndex = _usuarios.Count > 1 ? 1 : 0;
                cmbUsuarioSugerencias.SelectedIndex = 0;
                cmbUsuarioIntereses.SelectedIndex = 0;
            }
            else
            {
                txtUsuarioSeleccionado.Text = "Usuario seleccionado: --";
                txtUsuarioIntereses.Text = "Seleccione un usuario.";
                lstSugerencias.ItemsSource = null;
                lstIntereses.ItemsSource = null;
            }
        }

        private void CargarInteresesUsuarioSeleccionado(Usuario usuario)
        {
            txtUsuarioIntereses.Text = usuario.Nombre_Usuario;
            lstIntereses.ItemsSource = _catalogoIntereses.Select(interes => new InteresSeleccionable
            {
                Id = interes.Id,
                Nombre = interes.Nombre,
                Seleccionado = _interesesUsuarios.TryGetValue(usuario.Id_Usuario, out var interesesUsuario) &&
                               interesesUsuario.Contains(interes.Nombre)
            }).ToList();

        }

        private void ActualizarSugerencias(Usuario usuario)
        {
            txtUsuarioSeleccionado.Text = $"Usuario seleccionado: {usuario.Nombre_Usuario}";

            var sugerencias = _grafo.SugerirAmigos(usuario)
                .Concat(ObtenerSugerenciasPorIntereses(usuario))
                .DistinctBy(sugerido => sugerido.Id_Usuario)
                .Take(5)
                .Select((sugerido, indice) => new SugerenciaUsuario
                {
                    Usuario = sugerido,
                    Texto = $"{indice + 1}. {sugerido.Nombre_Usuario}{ObtenerTextoInteresesComunes(usuario, sugerido)}"
                })
                .ToList();

            lstSugerencias.ItemsSource = sugerencias.Count > 0
                ? sugerencias
                : new List<string> { "Sin sugerencias disponibles" };
        }

        private void DibujarGrafo()
        {
            lienzoGrafo.Children.Clear();
            if (lienzoGrafo.Visibility != Visibility.Visible)
            {
                return;
            }

            if (_usuarios.Count == 0)
            {
                return;
            }

            var ancho = lienzoGrafo.ActualWidth > 0 ? lienzoGrafo.ActualWidth : 700;
            var alto = lienzoGrafo.ActualHeight > 0 ? lienzoGrafo.ActualHeight : 360;
            var centroX = ancho / 2;
            var centroY = alto / 2;
            const double anchoTarjeta = 172;
            const double altoTarjeta = 68;
            var radioX = Math.Max(120, (ancho - anchoTarjeta - 90) / 2);
            var radioY = Math.Max(90, (alto - altoTarjeta - 80) / 2);

            for (var i = 0; i < _usuarios.Count; i++)
            {
                if (_posicionesGrafo.ContainsKey(_usuarios[i].Id_Usuario))
                {
                    continue;
                }

                var angulo = 2 * Math.PI * i / _usuarios.Count;
                var x = centroX + radioX * Math.Cos(angulo);
                var y = centroY + radioY * Math.Sin(angulo);

                _posicionesGrafo[_usuarios[i].Id_Usuario] = new Point(
                    Math.Clamp(x, anchoTarjeta / 2 + 10, ancho - anchoTarjeta / 2 - 10),
                    Math.Clamp(y, altoTarjeta / 2 + 10, alto - altoTarjeta / 2 - 10));
            }

            foreach (var relacion in _relaciones)
            {
                if (!_posicionesGrafo.TryGetValue(relacion.Seguidor, out var origen) ||
                    !_posicionesGrafo.TryGetValue(relacion.Seguido, out var destino))
                {
                    continue;
                }

                lienzoGrafo.Children.Add(new Line
                {
                    Tag = (relacion.Seguidor, relacion.Seguido),
                    X1 = origen.X,
                    Y1 = origen.Y,
                    X2 = destino.X,
                    Y2 = destino.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(88, 101, 116)),
                    StrokeThickness = 2.4,
                    Opacity = 0.86
                }); 
            }

            foreach (var usuario in _usuarios)
            {
                var posicion = _posicionesGrafo[usuario.Id_Usuario];
                var tarjeta = CrearTarjetaUsuario(usuario, anchoTarjeta, altoTarjeta);

                Canvas.SetLeft(tarjeta, posicion.X - anchoTarjeta / 2);
                Canvas.SetTop(tarjeta, posicion.Y - altoTarjeta / 2);
                lienzoGrafo.Children.Add(tarjeta);
            }
        }

        private Border CrearTarjetaUsuario(Usuario usuario, double anchoTarjeta, double altoTarjeta)
        {
            var bordeColor = usuario.Genero_Usuario.Equals("Femenino", StringComparison.OrdinalIgnoreCase)
                ? Color.FromRgb(125, 103, 84)
                : Color.FromRgb(15, 118, 110);

            var fondoColor = usuario.Genero_Usuario.Equals("Femenino", StringComparison.OrdinalIgnoreCase)
                ? Color.FromRgb(248, 246, 243)
                : Color.FromRgb(239, 247, 246);

            var tarjeta = new Border
            {
                Width = anchoTarjeta,
                Height = altoTarjeta,
                Background = new SolidColorBrush(fondoColor),
                BorderBrush = new SolidColorBrush(bordeColor),
                BorderThickness = new Thickness(1.2),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(7),
                ToolTip = $"{usuario.Nombre_Usuario}\n{usuario.Email_Usuario}",
                Tag = usuario,
                Cursor = Cursors.SizeAll
            };

            tarjeta.MouseLeftButtonDown += TarjetaUsuario_MouseLeftButtonDown;
            tarjeta.MouseMove += TarjetaUsuario_MouseMove;
            tarjeta.MouseLeftButtonUp += TarjetaUsuario_MouseLeftButtonUp;

            var sombra = new System.Windows.Media.Effects.DropShadowEffect
            {
                BlurRadius = 8,
                ShadowDepth = 1,
                Opacity = 0.12,
                Color = Colors.Black
            };
            tarjeta.Effect = sombra;

            var contenido = new Grid();
            contenido.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48) });
            contenido.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var avatar = new Border
            {
                Width = 42,
                Height = 42,
                CornerRadius = new CornerRadius(21),
                Background = new ImageBrush
                {
                    ImageSource = ObtenerImagenAvatar(usuario),
                    Stretch = Stretch.UniformToFill
                },
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(avatar, 0);
            contenido.Children.Add(avatar);

            var textos = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 0, 0)
            };

            textos.Children.Add(new TextBlock
            {
                Text = usuario.Nombre_Usuario,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(23, 32, 51)),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            textos.Children.Add(new TextBlock
            {
                Text = usuario.Genero_Usuario,
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush(Color.FromRgb(88, 101, 116))
            });

            textos.Children.Add(new TextBlock
            {
                Text = usuario.Email_Usuario,
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(101, 113, 128)),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            Grid.SetColumn(textos, 1);
            contenido.Children.Add(textos);

            tarjeta.Child = contenido;
            return tarjeta;
        }

        private void TarjetaUsuario_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border tarjeta)
            {
                return;
            }

            _tarjetaArrastrando = tarjeta;
            var posicionMouse = e.GetPosition(lienzoGrafo);
            _offsetArrastre = new Point(
                posicionMouse.X - Canvas.GetLeft(tarjeta),
                posicionMouse.Y - Canvas.GetTop(tarjeta));

            tarjeta.CaptureMouse();
            e.Handled = true;
        }

        private void TarjetaUsuario_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tarjetaArrastrando is null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (_tarjetaArrastrando.Tag is not Usuario usuario)
            {
                return;
            }

            var posicionMouse = e.GetPosition(lienzoGrafo);
            var izquierda = posicionMouse.X - _offsetArrastre.X;
            var arriba = posicionMouse.Y - _offsetArrastre.Y;

            izquierda = Math.Max(0, Math.Min(izquierda, lienzoGrafo.ActualWidth - _tarjetaArrastrando.Width));
            arriba = Math.Max(0, Math.Min(arriba, lienzoGrafo.ActualHeight - _tarjetaArrastrando.Height));

            Canvas.SetLeft(_tarjetaArrastrando, izquierda);
            Canvas.SetTop(_tarjetaArrastrando, arriba);

            _posicionesGrafo[usuario.Id_Usuario] = new Point(
                izquierda + _tarjetaArrastrando.Width / 2,
                arriba + _tarjetaArrastrando.Height / 2);

            ActualizarLineasGrafo();
            e.Handled = true;
        }

        private void TarjetaUsuario_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _tarjetaArrastrando?.ReleaseMouseCapture();
            _tarjetaArrastrando = null;
            e.Handled = true;
        }

        private void ActualizarLineasGrafo()
        {
            foreach (var linea in lienzoGrafo.Children.OfType<Line>())
            {
                if (linea.Tag is not ValueTuple<int, int> relacion ||
                    !_posicionesGrafo.TryGetValue(relacion.Item1, out var origen) ||
                    !_posicionesGrafo.TryGetValue(relacion.Item2, out var destino))
                {
                    continue;
                }

                linea.X1 = origen.X;
                linea.Y1 = origen.Y;
                linea.X2 = destino.X;
                linea.Y2 = destino.Y;
            }
        }

        private void lienzoGrafo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_tarjetaArrastrando is not null || _redibujandoPorTamano || _usuarios.Count == 0)
            {
                return;
            }

            _redibujandoPorTamano = true;
            _posicionesGrafo.Clear();
            DibujarGrafo();
            _redibujandoPorTamano = false;
        }

        private ImageSource ObtenerImagenAvatar(Usuario usuario)
        {
            try
            {
                string archivo = usuario.Genero_Usuario == "Femenino"
                    ? "mujer.png"
                    : "hombre.png";

                string ruta = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    archivo);

                if (!File.Exists(ruta))
                    return new BitmapImage();

                return new BitmapImage(new Uri(ruta, UriKind.Absolute));
            }
            catch
            {
                return new BitmapImage();
            }
        }

        private Usuario? ObtenerUsuarioSeleccionado()
        {
            return dgUsuarios.SelectedItem as Usuario;
        }

        private Usuario? ObtenerUsuarioSugerenciasSeleccionado()
        {
            return cmbUsuarioSugerencias.SelectedItem as Usuario ?? ObtenerUsuarioSeleccionado();
        }

        private Usuario? ObtenerUsuarioInteresesSeleccionado()
        {
            return cmbUsuarioIntereses.SelectedItem as Usuario ?? ObtenerUsuarioSeleccionado();
        }

        private void SeleccionarUsuarioEnCombos(int idUsuario)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Id_Usuario == idUsuario);
            if (usuario is null)
            {
                return;
            }

            dgUsuarios.SelectedItem = usuario;
            cmbSeguidorRelacion.SelectedItem = usuario;
            cmbUsuarioSugerencias.SelectedItem = usuario;
            cmbUsuarioIntereses.SelectedItem = usuario;
        }

        private string ObtenerGeneroSeleccionado()
        {
            return (cmbGenero.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Masculino";
        }

        private void LimpiarFormulario()
        {
            txtNombre.Clear();
            txtEmail.Clear();
            cmbGenero.SelectedIndex = 0;
            txtNombre.Focus();
        }

        private static void MostrarRecorrido(string tipoRecorrido, List<Usuario> recorrido)
        {
            var nombres = string.Join(" -> ", recorrido.Select(usuario => usuario.Nombre_Usuario));
            MessageBox.Show(nombres, $"Recorrido {tipoRecorrido}", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static bool NombreEsValido(string nombre)
        {
            return Regex.IsMatch(nombre, @"^[\p{L}]+(?: [\p{L}]+)*$");
        }

        private static bool EmailEsValido(string email)
        {
            return Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
        }

        private IEnumerable<Usuario> ObtenerSugerenciasPorIntereses(Usuario usuario)
        {
            if (!_interesesUsuarios.TryGetValue(usuario.Id_Usuario, out var interesesUsuario))
            {
                return [];
            }

            return _usuarios
                .Where(candidato => candidato.Id_Usuario != usuario.Id_Usuario)
                .Where(candidato => !_relaciones.Contains((usuario.Id_Usuario, candidato.Id_Usuario)))
                .Where(candidato => _interesesUsuarios.TryGetValue(candidato.Id_Usuario, out var interesesCandidato) &&
                                    interesesUsuario.Intersect(interesesCandidato).Any())
                .OrderByDescending(candidato => interesesUsuario.Intersect(_interesesUsuarios[candidato.Id_Usuario]).Count())
                .ThenBy(candidato => candidato.Nombre_Usuario);
        }

        private string ObtenerTextoInteresesComunes(Usuario usuario, Usuario sugerido)
        {
            if (!_interesesUsuarios.TryGetValue(usuario.Id_Usuario, out var interesesUsuario) ||
                !_interesesUsuarios.TryGetValue(sugerido.Id_Usuario, out var interesesSugerido))
            {
                return string.Empty;
            }

            var comunes = interesesUsuario.Intersect(interesesSugerido).Take(2).ToList();
            return comunes.Count == 0 ? string.Empty : $" ({string.Join(", ", comunes)})";
        }

        private bool ValidarConexionBaseDatos()
        {
            if (_baseDatosConectada)
            {
                return true;
            }

            MessageBox.Show("No hay conexion con la base de datos. Presione Cargar datos despues de encender MySQL/MariaDB.", "Base de datos desconectada", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void ActivarSeccion(Button botonActivo, Border panelActivo)
        {
            var estiloActivo = (Style)FindResource("SidebarActiveButtonStyle");
            var estiloNormal = (Style)FindResource("SidebarButtonStyle");

            btnSeccionUsuarios.Style = estiloNormal;
            btnSeccionRelaciones.Style = estiloNormal;
            btnSeccionGrafo.Style = estiloNormal;
            btnSeccionSugerencias.Style = estiloNormal;
            btnSeccionHistorial.Style = estiloNormal;
            botonActivo.Style = estiloActivo;

            MostrarPanel(panelActivo);
        }

        private void MostrarPanel(Border panelActivo)
        {
            panelNuevoUsuario.Visibility = Visibility.Collapsed;
            panelUsuariosActivos.Visibility = Visibility.Collapsed;
            panelIntereses.Visibility = Visibility.Collapsed;
            panelVisualizacionGrafo.Visibility = Visibility.Collapsed;
            panelSugerencias.Visibility = Visibility.Collapsed;
            panelHistorial.Visibility = Visibility.Collapsed;
            panelRelaciones.Visibility = Visibility.Collapsed;

            RestablecerPanel(panelNuevoUsuario);
            RestablecerPanel(panelUsuariosActivos);
            RestablecerPanel(panelIntereses);
            RestablecerPanel(panelVisualizacionGrafo);
            RestablecerPanel(panelSugerencias);
            RestablecerPanel(panelHistorial);
            RestablecerPanel(panelRelaciones);

            panelActivo.Visibility = Visibility.Visible;
            panelActivo.BorderBrush = new SolidColorBrush(Color.FromRgb(15, 118, 110));
            panelActivo.BorderThickness = new Thickness(2);
            panelActivo.BringIntoView();

            if (panelActivo == panelVisualizacionGrafo)
            {
                DibujarGrafo();
            }
        }

        private void ActivarBotonVista(Button botonActivo)
        {
            var estiloActivo = (Style)FindResource("PrimaryButtonStyle");

            RestablecerBotonesVista();
            botonActivo.Style = estiloActivo;
        }

        private void RestablecerBotonesVista()
        {
            var estiloNormal = (Style)FindResource("SecondaryButtonStyle");

            btnVistaNuevoUsuario.Style = estiloNormal;
            btnVistaUsuariosActivos.Style = estiloNormal;
            btnVistaSugerencias.Style = estiloNormal;
            btnVistaRelaciones.Style = estiloNormal;
            btnVistaGustos.Style = estiloNormal;
            btnVistaHistorial.Style = estiloNormal;
        }

        private static void RestablecerPanel(Border panel)
        {
            panel.BorderBrush = new SolidColorBrush(Color.FromRgb(216, 222, 230));
            panel.BorderThickness = new Thickness(1);
        }

        private static Modelos.Usuario ConvertirUsuarioModelo(Usuario usuario)
        {
            return new Modelos.Usuario(
                usuario.Id_Usuario,
                usuario.Nombre_Usuario,
                usuario.Genero_Usuario,
                usuario.Email_Usuario);
        }

        private void RegistrarCambio(string tipo, string detalle)
        {
            _historialCambios.Insert(0, new CambioHistorial
            {
                Fecha = DateTime.Now,
                Tipo = tipo,
                Detalle = detalle
            });
        }

        private void RegistrarCambioGustos(Usuario usuario, List<string> agregados, List<string> quitados)
        {
            if (agregados.Count == 0 && quitados.Count == 0)
            {
                RegistrarCambio("Gustos sin cambios", $"No se cambiaron los gustos de {usuario.Nombre_Usuario}.");
                return;
            }

            var partes = new List<string>();
            if (agregados.Count > 0)
            {
                partes.Add($"agregados: {string.Join(", ", agregados)}");
            }

            if (quitados.Count > 0)
            {
                partes.Add($"quitados: {string.Join(", ", quitados)}");
            }

            RegistrarCambio("Gustos actualizados", $"{usuario.Nombre_Usuario}: {string.Join("; ", partes)}.");
        }

        private class InteresSeleccionable
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public bool Seleccionado { get; set; }
        }

        private class SugerenciaUsuario
        {
            public Usuario Usuario { get; set; } = null!;
            public string Texto { get; set; } = string.Empty;

            public override string ToString()
            {
                return Texto;
            }
        }

        private class CambioHistorial
        {
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; } = string.Empty;
            public string Detalle { get; set; } = string.Empty;
            public string HoraTexto => Fecha.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void ActualizarEstadoControles()
        {
            bool habilitado = _baseDatosConectada;

            panelNuevoUsuario.IsEnabled = habilitado;
            panelRelaciones.IsEnabled = habilitado;
            panelIntereses.IsEnabled = habilitado;
            panelSugerencias.IsEnabled = habilitado;

            btnSeccionUsuarios.IsEnabled = habilitado;
            btnSeccionRelaciones.IsEnabled = habilitado;
            btnSeccionSugerencias.IsEnabled = habilitado;

            btnVistaNuevoUsuario.IsEnabled = habilitado;
            btnVistaUsuariosActivos.IsEnabled = habilitado;
            btnVistaRelaciones.IsEnabled = habilitado;
            btnVistaSugerencias.IsEnabled = habilitado;
            btnVistaGustos.IsEnabled = habilitado;
        }
    }
}
