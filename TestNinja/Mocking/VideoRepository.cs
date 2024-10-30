using System.Collections.Generic;
using System.Linq;

namespace TestNinja.Mocking
{
    public interface IVideoRepository
    {
        IEnumerable<Video> GetUnprocessedVideos();
    }

    public class VideoRepository : IVideoRepository
    {
        public IEnumerable<Video> GetUnprocessedVideos()
        {
            using var context = new VideoContext();
            var videos = context.Videos.Where(video => !video.IsProcessed).ToList();

            return videos;
        }
    }
}
