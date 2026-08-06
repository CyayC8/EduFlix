// Haalt de duur en een thumbnail-frame uit een gekozen videobestand,
// helemaal in de browser (geen server-verwerking/FFmpeg nodig).
window.eduflixVideoCapture = {
    capture: function (inputId) {
        return new Promise((resolve, reject) => {
            const input = document.getElementById(inputId);
            const file = input?.files?.[0];
            if (!file) {
                reject("geen bestand geselecteerd");
                return;
            }

            const url = URL.createObjectURL(file);
            const video = document.createElement("video");
            video.preload = "metadata";
            video.muted = true;
            video.src = url;

            video.onloadedmetadata = () => {
                video.currentTime = Math.min(1, video.duration / 2);
            };

            video.onseeked = () => {
                const canvas = document.createElement("canvas");
                canvas.width = video.videoWidth;
                canvas.height = video.videoHeight;
                canvas.getContext("2d").drawImage(video, 0, 0, canvas.width, canvas.height);

                canvas.toBlob(blob => {
                    const reader = new FileReader();
                    reader.onloadend = () => {
                        URL.revokeObjectURL(url);
                        resolve({
                            durationSeconds: Math.round(video.duration),
                            thumbnailBase64: reader.result.split(",")[1]
                        });
                    };
                    reader.readAsDataURL(blob);
                }, "image/jpeg", 0.8);
            };

            video.onerror = () => {
                URL.revokeObjectURL(url);
                reject("kon video niet laden voor thumbnail-extractie");
            };
        });
    }
};
