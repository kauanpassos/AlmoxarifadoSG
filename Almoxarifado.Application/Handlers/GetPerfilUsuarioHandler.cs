using MediatR;
using Google.Cloud.Firestore;

namespace Almoxarifado.Application.Queries;

public class GetPerfilUsuarioHandler : IRequestHandler<GetPerfilUsuarioQuery, PerfilUsuarioDto?>
{
    private readonly FirestoreDb _firestoreDb;

    public GetPerfilUsuarioHandler(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<PerfilUsuarioDto?> Handle(GetPerfilUsuarioQuery request, CancellationToken cancellationToken)
    {
        var docRef = _firestoreDb.Collection("Usuarios").Document(request.Uid);
        var snapshot = await docRef.GetSnapshotAsync(cancellationToken);

        if (!snapshot.Exists)
        {
            return null;
        }

        return new PerfilUsuarioDto
        {
            Id = request.Uid,
            Nome = snapshot.TryGetValue("Nome", out string nome) ? nome : "Usuário",
            Email = snapshot.TryGetValue("Email", out string email) ? email : string.Empty,
            Setor = snapshot.TryGetValue("Setor", out string setor) ? setor : "Não Informado",
            Tipo = snapshot.TryGetValue("Tipo", out string tipo) ? tipo : "Colaborador"
        };
    }
}