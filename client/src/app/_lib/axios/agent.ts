import axios from "axios";

const sleep = (delay: number) => {
  return new Promise(resolve => {
    setTimeout(resolve, delay);
  });
};

export const agent = axios.create({ baseURL: "http://localhost:3000" });

agent.interceptors.response.use(async response => {
  try {
    await sleep(1000);
    return response;
  } catch (error) {
    console.log(error);
    return Promise.reject(error);
  }
});
