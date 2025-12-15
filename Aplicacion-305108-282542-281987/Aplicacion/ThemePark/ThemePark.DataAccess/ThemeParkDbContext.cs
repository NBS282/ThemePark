using Microsoft.EntityFrameworkCore;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Entities.Tickets;

namespace ThemePark.DataAccess;

public class ThemeParkDbContext(DbContextOptions<ThemeParkDbContext> options) : DbContext(options)
{
    public DbSet<SystemDateTime> SystemDateTimes { get; set; }
    public DbSet<Attraction> Attractions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<ScoringStrategy> ScoringStrategies { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Entities.Roles.Rol> Roles { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<Configuracion> Configuraciones { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<RewardExchange> RewardExchanges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureSystemDateTime(modelBuilder);
        ConfigureAttraction(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureVisit(modelBuilder);
        ConfigureSession(modelBuilder);
        ConfigureScoringStrategy(modelBuilder);
        ConfigureConfiguracion(modelBuilder);
        ConfigureEvent(modelBuilder);
        ConfigureTicket(modelBuilder);
        ConfigureRoles(modelBuilder);
        ConfigureMaintenance(modelBuilder);
        ConfigureReward(modelBuilder);
        ConfigureRewardExchange(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureSystemDateTime(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemDateTime>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentDateTime).IsRequired(false);
        });
    }

    private static void ConfigureAttraction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.HasKey(e => e.Nombre);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Tipo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(500).IsRequired();
            entity.Property(e => e.EdadMinima).IsRequired();
            entity.Property(e => e.CapacidadMaxima).IsRequired();
            entity.Property(e => e.AforoActual).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.Property(e => e.FechaModificacion).IsRequired(false);
            entity.Property(e => e.TieneIncidenciaActiva).IsRequired();
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Contraseña).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FechaNacimiento).IsRequired();
            entity.HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId);
            entity.Property(e => e.FechaRegistro).IsRequired();
        });
    }

    private static void ConfigureVisit(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.AttractionName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.EntryTime).IsRequired();
            entity.Property(e => e.ExitTime).IsRequired(false);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.Points).IsRequired();
        });
    }

    private static void ConfigureSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
        });
    }

    private static void ConfigureScoringStrategy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScoringStrategy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Type).IsRequired(false)
                .HasConversion<string>();
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.PluginTypeIdentifier).IsRequired(false).HasMaxLength(200);
            entity.Property(e => e.ConfigurationJson).IsRequired(false);
            entity.HasOne(e => e.ConfiguracionTyped).WithOne().HasForeignKey<ScoringStrategy>("ConfiguracionId").IsRequired(false);
        });
    }

    private static void ConfigureConfiguracion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Configuraciones");
        });

        modelBuilder.Entity<ConfiguracionPorCombo>(entity =>
        {
            entity.Property(e => e.VentanaTemporalMinutos).IsRequired();
            entity.Property(e => e.BonusMultiplicador).IsRequired();
            entity.Property(e => e.MinimoAtracciones).IsRequired();
            entity.ToTable("ConfiguracionesPorCombo");
        });

        modelBuilder.Entity<ConfiguracionPorAtraccion>(entity =>
        {
            entity.Property(e => e.Valores)
                .HasColumnType("TEXT")
                .HasConversion(
#pragma warning disable IDE0028 // Collection initialization can be simplified
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, int>())
#pragma warning restore IDE0028 // Collection initialization can be simplified
                .IsRequired();
            entity.ToTable("ConfiguracionesPorAtraccion");
        });

        modelBuilder.Entity<ConfiguracionPorEvento>(entity =>
        {
            entity.Property(e => e.Puntos).IsRequired();
            entity.Property(e => e.Evento).IsRequired().HasMaxLength(200);
            entity.ToTable("ConfiguracionesPorEvento");
        });
    }

    private static void ConfigureEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.Hora).IsRequired();
            entity.Property(e => e.Aforo).IsRequired();
            entity.Property(e => e.CostoAdicional).IsRequired().HasColumnType("decimal(18,2)");
            entity.HasMany(e => e.Atracciones)
                .WithMany()
                .UsingEntity(j => j.ToTable("EventAttractions"));
        });
    }

    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoQR).IsRequired();
            entity.HasIndex(e => e.CodigoQR).IsUnique();
            entity.Property(e => e.FechaVisita).IsRequired();
            entity.Property(e => e.CodigoIdentificacionUsuario).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FechaCompra).IsRequired();

            entity.HasDiscriminator<string>("TipoTicket")
                .HasValue<GeneralTicket>("General")
                .HasValue<EventTicket>("Evento");
        });

        modelBuilder.Entity<EventTicket>(entity =>
        {
            entity.Property(e => e.EventoId).IsRequired();
        });
    }

    private static void ConfigureRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.Roles.Rol>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Entities.Roles.Rol>()
            .Property(e => e.UserId)
            .IsRequired();

        modelBuilder.Entity<Entities.Roles.Rol>()
            .ToTable("Roles")
            .HasDiscriminator<string>("TipoRol");

        modelBuilder.Entity<RolVisitante>()
            .HasBaseType<Entities.Roles.Rol>();

        modelBuilder.Entity<RolAdministradorParque>()
            .HasBaseType<Entities.Roles.Rol>();

        modelBuilder.Entity<RolOperadorAtraccion>()
            .HasBaseType<Entities.Roles.Rol>();

        modelBuilder.Entity<RolVisitante>()
            .Property(e => e.NivelMembresia)
            .IsRequired()
            .HasConversion<string>();
    }

    private static void ConfigureMaintenance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AttractionName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.HoraInicio).IsRequired();
            entity.Property(e => e.DuracionMinutos).IsRequired();
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IncidentId).IsRequired();
        });
    }

    private static void ConfigureReward(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CostoPuntos).IsRequired();
            entity.Property(e => e.CantidadDisponible).IsRequired();
            entity.Property(e => e.NivelMembresiaRequerido).IsRequired(false).HasConversion<string>();
            entity.Property(e => e.Activa).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
        });
    }

    private static void ConfigureRewardExchange(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RewardExchange>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RewardId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.PuntosDescontados).IsRequired();
            entity.Property(e => e.PuntosRestantesUsuario).IsRequired();
            entity.Property(e => e.FechaCanje).IsRequired();
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
        });
    }
}
