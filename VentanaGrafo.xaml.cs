using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Simulacion_de_red_social_utilizando_grafos.Clases;

namespace Simulacion_de_red_social_utilizando_grafos
{
    public partial class VentanaGrafo : Window
    {
        private readonly List<Usuario> _usuarios;
        private readonly HashSet<(int Seguidor, int Seguido)> _relaciones;
        private readonly HashSet<(int Origen, int Destino, string Interes)> _posiblesRelaciones;
        private readonly Dictionary<int, List<string>> _interesesUsuarios;
        private readonly Dictionary<int, Point> _posiciones = [];
        private readonly Dictionary<int, Border> _tarjetasUsuarios = [];
        private Border? _tarjetaArrastrando;
        private Point _offsetArrastre;
        private bool _actualizandoZoom;
        private int? _usuarioSeleccionadoId;

        public VentanaGrafo(
            IEnumerable<Usuario> usuarios,
            IEnumerable<(int Seguidor, int Seguido)> relaciones,
            IEnumerable<(int Origen, int Destino, string Interes)> posiblesRelaciones,
            Dictionary<int, List<string>> interesesUsuarios)
        {
            InitializeComponent();
            _usuarios = usuarios.ToList();
            _relaciones = relaciones.ToHashSet();
            _posiblesRelaciones = posiblesRelaciones.ToHashSet();
            _interesesUsuarios = interesesUsuarios;
            Loaded += VentanaGrafo_Loaded;
        }

        private void VentanaGrafo_Loaded(object sender, RoutedEventArgs e)
        {
            ActualizarListaUsuarios();
            DibujarGrafoCompleto();
        }

        private void btnReordenar_Click(object sender, RoutedEventArgs e)
        {
            _posiciones.Clear();
            DibujarGrafoCompleto();
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void sldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (escalaGrafo is null || _actualizandoZoom)
            {
                return;
            }

            AplicarZoom(e.NewValue);
        }

        private void btnAplicarZoom_Click(object sender, RoutedEventArgs e)
        {
            AplicarZoomDesdeTexto();
        }

        private void btnZoom100_Click(object sender, RoutedEventArgs e)
        {
            AplicarZoom(1.0);
            CentrarUsuarioSeleccionado();
        }

        private void txtZoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AplicarZoomDesdeTexto();
                e.Handled = true;
            }
        }

        private void AplicarZoomDesdeTexto()
        {
            if (!double.TryParse(txtZoom.Text.Trim(), out var porcentaje))
            {
                MessageBox.Show("Ingrese un porcentaje de zoom valido. Ejemplo: 75, 100 o 150.", "Zoom", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtZoom.Text = Math.Round(escalaGrafo.ScaleX * 100).ToString();
                txtZoom.Focus();
                return;
            }

            porcentaje = Math.Clamp(porcentaje, 25, 200);
            AplicarZoom(porcentaje / 100);
        }

        private void AplicarZoom(double zoom)
        {
            zoom = Math.Clamp(zoom, sldZoom.Minimum, sldZoom.Maximum);

            _actualizandoZoom = true;
            escalaGrafo.ScaleX = zoom;
            escalaGrafo.ScaleY = zoom;
            sldZoom.Value = zoom;
            txtZoom.Text = Math.Round(zoom * 100).ToString();
            _actualizandoZoom = false;
        }

        private void txtBuscarUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarListaUsuarios();
            ActualizarTarjetasSeleccionadas();
        }

        private void lstUsuariosGrafo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstUsuariosGrafo.SelectedItem is not Usuario usuario)
            {
                _usuarioSeleccionadoId = null;
                ActualizarTarjetasSeleccionadas();
                return;
            }

            _usuarioSeleccionadoId = usuario.Id_Usuario;
            ActualizarTarjetasSeleccionadas();
            CentrarUsuarioSeleccionado();
        }

        private void ActualizarListaUsuarios()
        {
            var filtro = ObtenerFiltroBusqueda();
            var usuariosFiltrados = _usuarios
                .Where(usuario => UsuarioCoincideBusqueda(usuario, filtro))
                .OrderBy(usuario => usuario.Id_Usuario)
                .ToList();

            lstUsuariosGrafo.ItemsSource = usuariosFiltrados;
            txtCantidadUsuariosLista.Text = $"{usuariosFiltrados.Count} de {_usuarios.Count} usuarios";

            if (_usuarioSeleccionadoId is int idSeleccionado)
            {
                lstUsuariosGrafo.SelectedItem = usuariosFiltrados.FirstOrDefault(usuario => usuario.Id_Usuario == idSeleccionado);
            }
        }

        private void DibujarGrafoCompleto()
        {
            lienzoGrafoCompleto.Children.Clear();
            _tarjetasUsuarios.Clear();
            txtResumenGrafo.Text = $"Nodos: {_usuarios.Count} | Relaciones reales: {_relaciones.Count} | Posibles por gustos: {_posiblesRelaciones.Count}";

            if (_usuarios.Count == 0)
            {
                return;
            }

            const double anchoTarjeta = 190;
            const double altoTarjeta = 74;
            const double separacionRadio = 245;

            var anillos = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(_usuarios.Count / 6.0)));
            var radioMaximo = 260 + anillos * separacionRadio;
            var anchoCanvas = Math.Max(1600, radioMaximo * 2 + 520);
            var altoCanvas = Math.Max(1000, radioMaximo * 2 + 520);
            var centro = new Point(anchoCanvas / 2, altoCanvas / 2);

            lienzoGrafoCompleto.Width = anchoCanvas;
            lienzoGrafoCompleto.Height = altoCanvas;

            CalcularPosiciones(centro, separacionRadio, anchoTarjeta, altoTarjeta, anchoCanvas, altoCanvas);
            DibujarPosiblesRelaciones();
            DibujarRelaciones();
            DibujarUsuarios(anchoTarjeta, altoTarjeta);
            ActualizarTarjetasSeleccionadas();
        }

        private void CalcularPosiciones(Point centro, double separacionRadio, double anchoTarjeta, double altoTarjeta, double anchoCanvas, double altoCanvas)
        {
            if (_posiciones.Count == _usuarios.Count)
            {
                return;
            }

            var indiceUsuario = 0;
            var capacidadAnillo = 10;
            var anillo = 1;

            while (indiceUsuario < _usuarios.Count)
            {
                var restantes = _usuarios.Count - indiceUsuario;
                var cantidadEnAnillo = Math.Min(capacidadAnillo, restantes);
                var radio = anillo * separacionRadio;

                for (var i = 0; i < cantidadEnAnillo; i++)
                {
                    var usuario = _usuarios[indiceUsuario];
                    var angulo = 2 * Math.PI * i / cantidadEnAnillo;
                    var x = centro.X + radio * Math.Cos(angulo);
                    var y = centro.Y + radio * Math.Sin(angulo);

                    _posiciones[usuario.Id_Usuario] = new Point(
                        Math.Clamp(x, anchoTarjeta / 2 + 20, anchoCanvas - anchoTarjeta / 2 - 20),
                        Math.Clamp(y, altoTarjeta / 2 + 20, altoCanvas - altoTarjeta / 2 - 20));

                    indiceUsuario++;
                }

                capacidadAnillo += 12;
                anillo++;
            }
        }

        private void DibujarRelaciones()
        {
            foreach (var relacion in _relaciones)
            {
                if (!_posiciones.TryGetValue(relacion.Seguidor, out var origen) ||
                    !_posiciones.TryGetValue(relacion.Seguido, out var destino))
                {
                    continue;
                }

                lienzoGrafoCompleto.Children.Add(new Line
                {
                    Tag = relacion,
                    X1 = origen.X,
                    Y1 = origen.Y,
                    X2 = destino.X,
                    Y2 = destino.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(88, 101, 116)),
                    StrokeThickness = 2.8,
                    Opacity = 0.86
                });
            }
        }

        private void DibujarPosiblesRelaciones()
        {
            foreach (var relacion in _posiblesRelaciones)
            {
                if (!_posiciones.TryGetValue(relacion.Origen, out var origen) ||
                    !_posiciones.TryGetValue(relacion.Destino, out var destino))
                {
                    continue;
                }

                lienzoGrafoCompleto.Children.Add(new Line
                {
                    Tag = relacion,
                    X1 = origen.X,
                    Y1 = origen.Y,
                    X2 = destino.X,
                    Y2 = destino.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(15, 118, 110)),
                    StrokeThickness = 2.4,
                    StrokeDashArray = new DoubleCollection { 7, 5 },
                    Opacity = 0.72,
                    ToolTip = $"Posible relacion por gusto comun: {relacion.Interes}"
                });
            }
        }

        private void DibujarUsuarios(double anchoTarjeta, double altoTarjeta)
        {
            foreach (var usuario in _usuarios)
            {
                var posicion = _posiciones[usuario.Id_Usuario];
                var tarjeta = CrearTarjetaUsuario(usuario, anchoTarjeta, altoTarjeta);

                Canvas.SetLeft(tarjeta, posicion.X - anchoTarjeta / 2);
                Canvas.SetTop(tarjeta, posicion.Y - altoTarjeta / 2);
                lienzoGrafoCompleto.Children.Add(tarjeta);
            }
        }

        private Border CrearTarjetaUsuario(Usuario usuario, double anchoTarjeta, double altoTarjeta)
        {
            var bordeColor = ObtenerColorBordeUsuario(usuario);
            var fondoColor = ObtenerColorFondoUsuario(usuario);

            var tarjeta = new Border
            {
                Width = anchoTarjeta,
                Height = altoTarjeta,
                Background = new SolidColorBrush(fondoColor),
                BorderBrush = new SolidColorBrush(bordeColor),
                BorderThickness = new Thickness(1.2),
                CornerRadius = new CornerRadius(7),
                Padding = new Thickness(8),
                ToolTip = $"{usuario.Nombre_Usuario}\n{usuario.Email_Usuario}\nGustos: {ObtenerTextoIntereses(usuario)}",
                Tag = usuario,
                Cursor = Cursors.SizeAll,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 8,
                    ShadowDepth = 1,
                    Opacity = 0.12,
                    Color = Colors.Black
                }
            };

            tarjeta.MouseLeftButtonDown += TarjetaUsuario_MouseLeftButtonDown;
            tarjeta.MouseMove += TarjetaUsuario_MouseMove;
            tarjeta.MouseLeftButtonUp += TarjetaUsuario_MouseLeftButtonUp;
            tarjeta.MouseRightButtonDown += TarjetaUsuario_MouseRightButtonDown;

            var contenido = new Grid();
            contenido.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(52) });
            contenido.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var avatar = new Border
            {
                Width = 44,
                Height = 44,
                CornerRadius = new CornerRadius(22),
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
                Text = ObtenerTextoIntereses(usuario),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            Grid.SetColumn(textos, 1);
            contenido.Children.Add(textos);
            tarjeta.Child = contenido;
            _tarjetasUsuarios[usuario.Id_Usuario] = tarjeta;

            return tarjeta;
        }

        private void TarjetaUsuario_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border { Tag: Usuario usuario })
            {
                return;
            }

            SeleccionarUsuario(usuario);
            e.Handled = true;
        }

        private void SeleccionarUsuario(Usuario usuario)
        {
            _usuarioSeleccionadoId = usuario.Id_Usuario;
            var usuarioEnLista = lstUsuariosGrafo.Items
                .OfType<Usuario>()
                .FirstOrDefault(item => item.Id_Usuario == usuario.Id_Usuario);

            if (usuarioEnLista is not null)
            {
                lstUsuariosGrafo.SelectedItem = usuarioEnLista;
            }

            ActualizarTarjetasSeleccionadas();
        }

        private string ObtenerTextoIntereses(Usuario usuario)
        {
            return _interesesUsuarios.TryGetValue(usuario.Id_Usuario, out var intereses) && intereses.Count > 0
                ? string.Join(", ", intereses.Take(3))
                : "Sin gustos";
        }

        private void TarjetaUsuario_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border tarjeta)
            {
                return;
            }

            if (tarjeta.Tag is Usuario usuario)
            {
                SeleccionarUsuario(usuario);
            }

            _tarjetaArrastrando = tarjeta;
            var posicionMouse = e.GetPosition(lienzoGrafoCompleto);
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

            var posicionMouse = e.GetPosition(lienzoGrafoCompleto);
            var izquierda = posicionMouse.X - _offsetArrastre.X;
            var arriba = posicionMouse.Y - _offsetArrastre.Y;

            izquierda = Math.Max(0, Math.Min(izquierda, lienzoGrafoCompleto.Width - _tarjetaArrastrando.Width));
            arriba = Math.Max(0, Math.Min(arriba, lienzoGrafoCompleto.Height - _tarjetaArrastrando.Height));

            Canvas.SetLeft(_tarjetaArrastrando, izquierda);
            Canvas.SetTop(_tarjetaArrastrando, arriba);

            _posiciones[usuario.Id_Usuario] = new Point(
                izquierda + _tarjetaArrastrando.Width / 2,
                arriba + _tarjetaArrastrando.Height / 2);

            ActualizarLineas();
            e.Handled = true;
        }

        private void ActualizarTarjetasSeleccionadas()
        {
            var filtro = ObtenerFiltroBusqueda();
            var usuariosActivos = ObtenerIdsUsuariosActivos(filtro);
            var usuariosRelacionados = ObtenerIdsRelaciones(usuariosActivos);
            var usuariosSugeridos = ObtenerIdsSugerencias(usuariosActivos);

            foreach (var tarjeta in _tarjetasUsuarios.Values)
            {
                if (tarjeta.Tag is not Usuario usuario)
                {
                    continue;
                }

                var esSeleccionado = _usuarioSeleccionadoId == usuario.Id_Usuario;
                var coincideBusqueda = !string.IsNullOrWhiteSpace(filtro) && UsuarioCoincideBusqueda(usuario, filtro);
                var esActivo = usuariosActivos.Contains(usuario.Id_Usuario);
                var esRelacion = usuariosRelacionados.Contains(usuario.Id_Usuario);
                var esSugerencia = usuariosSugeridos.Contains(usuario.Id_Usuario);
                var borde = esSeleccionado || coincideBusqueda || esActivo
                    ? Color.FromRgb(245, 158, 11)
                    : esRelacion
                        ? Color.FromRgb(37, 99, 235)
                        : esSugerencia
                            ? Color.FromRgb(5, 150, 105)
                            : ObtenerColorBordeUsuario(usuario);
                var fondo = esSeleccionado || coincideBusqueda || esActivo
                    ? Color.FromRgb(254, 243, 199)
                    : esRelacion
                        ? Color.FromRgb(219, 234, 254)
                        : esSugerencia
                            ? Color.FromRgb(209, 250, 229)
                            : ObtenerColorFondoUsuario(usuario);

                tarjeta.Background = new SolidColorBrush(fondo);
                tarjeta.BorderBrush = new SolidColorBrush(borde);
                tarjeta.BorderThickness = new Thickness(esSeleccionado || coincideBusqueda || esActivo || esRelacion || esSugerencia ? 3 : 1.2);
            }

            ActualizarColoresLineas(usuariosActivos);
        }

        private string ObtenerFiltroBusqueda()
        {
            return txtBuscarUsuario?.Text.Trim() ?? string.Empty;
        }

        private static bool UsuarioCoincideBusqueda(Usuario usuario, string filtro)
        {
            return string.IsNullOrWhiteSpace(filtro) ||
                   usuario.Nombre_Usuario.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                   usuario.Email_Usuario.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                   usuario.Genero_Usuario.Contains(filtro, StringComparison.OrdinalIgnoreCase);
        }

        private HashSet<int> ObtenerIdsUsuariosActivos(string filtro)
        {
            if (_usuarioSeleccionadoId is int idSeleccionado)
            {
                return [idSeleccionado];
            }

            if (string.IsNullOrWhiteSpace(filtro))
            {
                return [];
            }

            return _usuarios
                .Where(usuario => UsuarioCoincideBusqueda(usuario, filtro))
                .Select(usuario => usuario.Id_Usuario)
                .ToHashSet();
        }

        private HashSet<int> ObtenerIdsRelaciones(HashSet<int> usuariosActivos)
        {
            if (usuariosActivos.Count == 0)
            {
                return [];
            }

            return _relaciones
                .Where(relacion => usuariosActivos.Contains(relacion.Seguidor) || usuariosActivos.Contains(relacion.Seguido))
                .SelectMany(relacion => new[] { relacion.Seguidor, relacion.Seguido })
                .Where(idUsuario => !usuariosActivos.Contains(idUsuario))
                .ToHashSet();
        }

        private HashSet<int> ObtenerIdsSugerencias(HashSet<int> usuariosActivos)
        {
            if (usuariosActivos.Count == 0)
            {
                return [];
            }

            return _posiblesRelaciones
                .Where(relacion => usuariosActivos.Contains(relacion.Origen) || usuariosActivos.Contains(relacion.Destino))
                .SelectMany(relacion => new[] { relacion.Origen, relacion.Destino })
                .Where(idUsuario => !usuariosActivos.Contains(idUsuario))
                .ToHashSet();
        }

        private void ActualizarColoresLineas(HashSet<int> usuariosActivos)
        {
            var hayUsuariosActivos = usuariosActivos.Count > 0;

            foreach (var linea in lienzoGrafoCompleto.Children.OfType<Line>())
            {
                if (!TryObtenerExtremosLinea(linea.Tag, out var origenId, out var destinoId))
                {
                    continue;
                }

                var estaConectada = hayUsuariosActivos && (usuariosActivos.Contains(origenId) || usuariosActivos.Contains(destinoId));

                if (linea.Tag is ValueTuple<int, int>)
                {
                    linea.Stroke = new SolidColorBrush(estaConectada ? Color.FromRgb(37, 99, 235) : Color.FromRgb(88, 101, 116));
                    linea.StrokeThickness = estaConectada ? 4.2 : 2.8;
                    linea.Opacity = hayUsuariosActivos ? estaConectada ? 1 : 0.36 : 0.86;
                    continue;
                }

                if (linea.Tag is ValueTuple<int, int, string>)
                {
                    linea.Stroke = new SolidColorBrush(estaConectada ? Color.FromRgb(5, 150, 105) : Color.FromRgb(15, 118, 110));
                    linea.StrokeThickness = estaConectada ? 4.0 : 2.4;
                    linea.Opacity = hayUsuariosActivos ? estaConectada ? 1 : 0.26 : 0.72;
                }
            }
        }

        private static Color ObtenerColorBordeUsuario(Usuario usuario)
        {
            return usuario.Genero_Usuario.Equals("Femenino", StringComparison.OrdinalIgnoreCase)
                ? Color.FromRgb(125, 103, 84)
                : Color.FromRgb(15, 118, 110);
        }

        private static Color ObtenerColorFondoUsuario(Usuario usuario)
        {
            return usuario.Genero_Usuario.Equals("Femenino", StringComparison.OrdinalIgnoreCase)
                ? Color.FromRgb(248, 246, 243)
                : Color.FromRgb(239, 247, 246);
        }

        private void CentrarUsuarioSeleccionado()
        {
            if (_usuarioSeleccionadoId is not int idUsuario ||
                !_posiciones.TryGetValue(idUsuario, out var posicion))
            {
                return;
            }

            var zoom = escalaGrafo.ScaleX;
            var offsetX = Math.Max(0, posicion.X * zoom - scrollGrafo.ViewportWidth / 2);
            var offsetY = Math.Max(0, posicion.Y * zoom - scrollGrafo.ViewportHeight / 2);

            scrollGrafo.ScrollToHorizontalOffset(offsetX);
            scrollGrafo.ScrollToVerticalOffset(offsetY);
        }

        private void TarjetaUsuario_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _tarjetaArrastrando?.ReleaseMouseCapture();
            _tarjetaArrastrando = null;
            e.Handled = true;
        }

        private void ActualizarLineas()
        {
            foreach (var linea in lienzoGrafoCompleto.Children.OfType<Line>())
            {
                if (!TryObtenerExtremosLinea(linea.Tag, out var origenId, out var destinoId) ||
                    !_posiciones.TryGetValue(origenId, out var origen) ||
                    !_posiciones.TryGetValue(destinoId, out var destino))
                {
                    continue;
                }

                linea.X1 = origen.X;
                linea.Y1 = origen.Y;
                linea.X2 = destino.X;
                linea.Y2 = destino.Y;
            }
        }

        private static bool TryObtenerExtremosLinea(object? tag, out int origen, out int destino)
        {
            if (tag is ValueTuple<int, int> relacionReal)
            {
                origen = relacionReal.Item1;
                destino = relacionReal.Item2;
                return true;
            }

            if (tag is ValueTuple<int, int, string> relacionPosible)
            {
                origen = relacionPosible.Item1;
                destino = relacionPosible.Item2;
                return true;
            }

            origen = 0;
            destino = 0;
            return false;
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
    }
}