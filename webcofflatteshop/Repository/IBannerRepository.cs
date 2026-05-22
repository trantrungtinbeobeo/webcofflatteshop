using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public interface IBannerRepository
{
    BannerSettings Get();
    void Save(BannerSettings settings);
}
