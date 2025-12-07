FROM ubuntu 

# Không cần tương tác khi apt install
ENV DEBIAN_FRONTEND=noninteractive

WORKDIR /app

RUN apt-get update
RUN apt-get install -y curl git build-essential ca-certificates

#install nodejs
RUN curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - \
     && apt-get install -y nodejs

#install angular
RUN npm i -g @angular/cli

COPY package*.json ./

RUN npm i

#COPY .. ./
RUN ng analytics off
# Expose port Angular dev server
EXPOSE 4300

#-host 0.0.0.0 : lắng nghe tất cả IP
# run at port 4300 
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "4300"]

