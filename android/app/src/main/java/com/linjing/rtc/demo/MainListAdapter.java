package com.linjing.rtc.demo;

import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;
import java.util.List;

public class MainListAdapter extends RecyclerView.Adapter<MainListAdapter.MainListViewHolder> {
    public static class MainData {
        private String itemName = "";
        public int actionKey = 0;
        public Class mClazz = null;

        public MainData(String itemName, int actionKey, Class clazz) {
            this.itemName = itemName;
            this.actionKey = actionKey;
            mClazz = clazz;

        }
    }
    private List<MainData> mData = new ArrayList<>();
    private OnItemClick mItemListener;

    @NonNull
    @Override
    public MainListViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View item = View.inflate(parent.getContext(), R.layout.item_main_list, null);
        return new MainListViewHolder(item);
    }

    @Override
    public void onBindViewHolder(@NonNull MainListViewHolder holder, int position) {
        holder.itemButton.setText(mData.get(position).itemName);
        final int position1 = position;
        holder.itemButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (mItemListener != null) {
                    mItemListener.onMainItemClick(mData.get(position1));
                }
            }
        });
    }

    @Override
    public int getItemCount() {
        return mData.size();
    }

    public void setData(@NonNull List<MainData> data) {
        mData = data;
    }

    public void setItemListener(@NonNull OnItemClick listener) {
        mItemListener = listener;

    }

    public interface OnItemClick {
        void onMainItemClick(MainData data);
    }


    public static class MainListViewHolder extends RecyclerView.ViewHolder {

        private Button itemButton;

        public MainListViewHolder(@NonNull View itemView) {
            super(itemView);
            itemButton = itemView.findViewById(R.id.item_button);
        }
    }

}
