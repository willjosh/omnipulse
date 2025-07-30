import React from "react";

const Loading = () => {
  return (
    <div className="bg-white flex justify-center items-center rounded-lg shadow overflow-hidden h-[500px] w-[1260px]">
      <div className="p-8">
        <div className="animate-spin rounded-full size-8 border-b-2 border-blue-600 mx-auto"></div>
        <p className="mt-2 text-gray-500">Loading...</p>
      </div>
    </div>
  );
};

export default Loading;
